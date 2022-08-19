using Newtonsoft.Json;
using NSubstitute;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Uqs.OpenWeather.Test.Unit;

public class ClientTests
{
    private const string ONECALL_BASE_URL = "https://api.openweathermap.org/data/2.5/onecall";
    private const string FAKE_KEY = "thisisafakeapikey";
    private const decimal GREENWICH_LATITUDE = 51.4769m;
    private const decimal GREENWICH_LONGITUDE = 0.0005m;

    [Fact]
    public async Task OneCallAsync_RequiredParametersOnlyLatLonAppId_UrlIsFormattedAsExpected()
    {
        // Arrange
        var httpResponseMessage = new HttpResponseMessage()
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent("{}")
        };
        var fakeHttpMessageHandler = Substitute.ForPartsOf<FakeHttpMessageHandler>(httpResponseMessage);
        HttpRequestMessage? actualHttpRequestMessage = null;
        fakeHttpMessageHandler.SendSpyAsync(Arg.Do<HttpRequestMessage>(x => actualHttpRequestMessage = x), Arg.Any<CancellationToken>()).Returns(Task.FromResult(httpResponseMessage));
        var fakeHttpClient = new HttpClient(fakeHttpMessageHandler);
        var client = new Client(FAKE_KEY, fakeHttpClient);

        // Act
        var oneCallResponse = await client.OneCallAsync(GREENWICH_LATITUDE, GREENWICH_LONGITUDE, new Excludes[0], Units.Standard);

        // Assert
        Assert.NotNull(actualHttpRequestMessage);
        Assert.NotNull(actualHttpRequestMessage!.RequestUri);
        string? actualUrl = actualHttpRequestMessage!.RequestUri!.AbsoluteUri.ToString();
        Assert.Contains(ONECALL_BASE_URL, actualUrl);
        Assert.Contains($"lat={GREENWICH_LATITUDE}", actualUrl);
        Assert.Contains($"lon={GREENWICH_LONGITUDE}", actualUrl);
        Assert.Contains($"appid={FAKE_KEY}", actualUrl);
        Assert.True(actualUrl!.Count(x => x == '&') >= 2); // we have at least three parameters with at least two &
    }

    [Theory]
    [InlineData(new Excludes[] {}, Units.Standard, null, "units=standard")]
    [InlineData(new Excludes[] {}, Units.Metric, null, "units=metric")]
    [InlineData(new Excludes[] {}, Units.Imperial, null, "units=imperial")]
    [InlineData(new Excludes[] { Excludes.Current }, Units.Standard, "exclude=current", "units=standard")]
    [InlineData(new Excludes[] { Excludes.Current, Excludes.Minutely }, Units.Standard, "exclude=current%2cminutely", "units=standard")]
    [InlineData(new Excludes[] { Excludes.Current, Excludes.Minutely, Excludes.Hourly, Excludes.Daily, Excludes.Alerts }, Units.Standard, "exclude=current%2cminutely%2chourly%2cdaily%2calerts", "units=standard")]
    [InlineData(new Excludes[] { Excludes.Current, Excludes.Minutely, Excludes.Hourly, Excludes.Alerts }, Units.Standard, "exclude=current%2cminutely%2chourly%2calerts", "units=standard")]
    public async Task OneCallAsync_WithRequiredAndOptional_UrlIsFormattedAsExpected(Excludes[] excludes, Units unit, string? excludesQuery, string unitQuery)
    {
        // Arrange
        var httpResponseMessage = new HttpResponseMessage()
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent("{}")
        };
        var fakeHttpMessageHandler = Substitute.ForPartsOf<FakeHttpMessageHandler>(httpResponseMessage);
        HttpRequestMessage? actualHttpRequestMessage = null;
        fakeHttpMessageHandler.SendSpyAsync(Arg.Do<HttpRequestMessage>(x => actualHttpRequestMessage = x), Arg.Any<CancellationToken>()).Returns(Task.FromResult(httpResponseMessage));
        var fakeHttpClient = new HttpClient(fakeHttpMessageHandler);
        var client = new Client(FAKE_KEY, fakeHttpClient);

        // Act
        var oneCallResponse = await client.OneCallAsync(GREENWICH_LATITUDE, GREENWICH_LONGITUDE, excludes ?? new Excludes[0], unit);

        // Assert
        Assert.NotNull(actualHttpRequestMessage);
        Assert.NotNull(actualHttpRequestMessage!.RequestUri);
        string? actualUrl = actualHttpRequestMessage!.RequestUri!.AbsoluteUri.ToString();
        if (excludesQuery == null)
        {
            Assert.DoesNotContain("exclude", actualUrl);
            Assert.True(actualUrl!.Count(x => x == '&') >= 3);
        }
        else
        {
            Assert.Contains(excludesQuery, actualUrl);
            Assert.True(actualUrl!.Count(x => x == '&') >= 4);
        }
        Assert.Contains(unitQuery, actualUrl);
    }

    [Fact]
    public async Task OneCallAsync_UsingDailySampleFile_SerializedProperly()
    {
        // Arrange
        var sampleFilePath = "Samples/onecall-daily.json";
        string sampleContent = await File.ReadAllTextAsync(sampleFilePath);
        var httpResponseMessage = new HttpResponseMessage()
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(sampleContent)
        };
        var fakeHttpMessageHandler = new FakeHttpMessageHandler(httpResponseMessage);
        var fakeHttpClient = new HttpClient(fakeHttpMessageHandler);
        var client = new Client(FAKE_KEY, fakeHttpClient);

        // Act
        var oneCallResponse = await client.OneCallAsync(GREENWICH_LATITUDE, GREENWICH_LONGITUDE, new Excludes[] { Excludes.Current, Excludes.Minutely, Excludes.Hourly, Excludes.Alerts }, Units.Imperial);

        // Assert
        Assert.Equal(8, oneCallResponse.Daily.Length);
        Assert.NotNull(oneCallResponse.Daily[0].Temp);
        Assert.Equal(6.96, oneCallResponse.Daily[0].Temp.Day);
        Assert.NotNull(oneCallResponse.Daily[1].Temp);
        Assert.Equal(6.73, oneCallResponse.Daily[1].Temp.Day);
        Assert.NotNull(oneCallResponse.Daily[2].Temp);
        Assert.Equal(5.31, oneCallResponse.Daily[2].Temp.Day);
        Assert.NotNull(oneCallResponse.Daily[3].Temp);
        Assert.Equal(5.66, oneCallResponse.Daily[3].Temp.Day);
        Assert.NotNull(oneCallResponse.Daily[4].Temp);
        Assert.Equal(3.88, oneCallResponse.Daily[4].Temp.Day);
    }

    [Fact]
    public async Task OneCallAsync_NonSerializableTextContent_JsonReaderException()
    {
        // Arrange
        var httpResponseMessage = new HttpResponseMessage()
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent("Garbage That is not JSON")
        };
        var fakeHttpMessageHandler = new FakeHttpMessageHandler(httpResponseMessage);
        var fakeHttpClient = new HttpClient(fakeHttpMessageHandler);
        var client = new Client(FAKE_KEY, fakeHttpClient);

        // Act
        Exception ex = await Record.ExceptionAsync(() => _ = client.OneCallAsync(GREENWICH_LATITUDE, GREENWICH_LONGITUDE, new Excludes[0], Units.Imperial));

        // Assert
        Assert.IsType<JsonReaderException>(ex);
    }

    [Fact]
    public async Task OneCallAsync_EmptyContent_InvalidOperationException()
    {
        // Arrange
        var httpResponseMessage = new HttpResponseMessage()
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent("")
        };
        var fakeHttpMessageHandler = new FakeHttpMessageHandler(httpResponseMessage);
        var fakeHttpClient = new HttpClient(fakeHttpMessageHandler);
        var client = new Client(FAKE_KEY, fakeHttpClient);

        // Act
        Exception ex = await Record.ExceptionAsync(() => _ = client.OneCallAsync(GREENWICH_LATITUDE, GREENWICH_LONGITUDE, new Excludes[0], Units.Imperial));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
    }

}