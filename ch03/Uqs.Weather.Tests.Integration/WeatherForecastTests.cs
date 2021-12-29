using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using System.Net.Http.Json;

namespace Uqs.Weather.Tests.Integration;

public class WeatherForecastTests
{
    private const string BASE_ADDRESS = "https://localhost:7218";
    private const string API_URI = "/WeatherForecast/GetRealWeatherForecast";
    private record WeatherForecast(DateTime Date, int TemperatureC, int TemperatureF, string? Summary);

    // You have to run Uqs.Weather before running this test
    [Fact]
    public async Task GetRealWeatherForecast_Execute_GetNext5Days()
    {
        // Arrange
        var today = DateTime.Now.Date;
        var next5Days = new[] { today.AddDays(1), today.AddDays(2), 
            today.AddDays(3), today.AddDays(4), today.AddDays(5) };
        HttpClient httpClient = new HttpClient
        { BaseAddress = new Uri(BASE_ADDRESS) };

        // Act
        var httpRes = await httpClient.GetAsync(API_URI);

        // Assert
        var wfs = await httpRes.Content.ReadFromJsonAsync<WeatherForecast[]>();
        for (int i = 0;i < 5;i++)
        {
            Assert.Equal(next5Days[i], wfs[i].Date.Date);
        }
    }

    
}