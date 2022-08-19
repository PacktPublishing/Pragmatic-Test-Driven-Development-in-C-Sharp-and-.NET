using NSubstitute;
using Uqs.WeatherForecaster.Services;

namespace Uqs.WeatherForecaster.Tests.Unit;

public class WeatherAnalysisServiceTests
{
    private IOpenWeatherService _openWeatherServiceMock = Substitute.For<IOpenWeatherService>();
    private WeatherAnalysisService _sut;
    private const decimal LAT = 2.2m;
    private const decimal LON = 1.1m;

    public WeatherAnalysisServiceTests()
    {
        _sut = new (_openWeatherServiceMock);
    }

    [Fact]
    public async Task GetForecastWeatherAnalysis_LatAndLonPassed_ReceivedByOpenWeatherAccurately()
    {
        // Arrange
        decimal actualLat = 0;
        decimal actualLon = 0;
        _openWeatherServiceMock.OneCallAsync(
            Arg.Do<decimal>(x => actualLat = x),
            Arg.Do<decimal>(x => actualLon = x),
            Arg.Any<IEnumerable<Excludes>>(), 
            Arg.Any<Units>())
            .Returns(Task.FromResult(GetSample(_defaultTemps)));

        // Act
        await _sut.GetForecastWeatherAnalysis(LAT, LON);

        // Assert
        Assert.Equal(LAT, actualLat);
        Assert.Equal(LON, actualLon);
    }

    [Fact]
    public async Task GetForecastWeatherAnalysis_NoParemetersOverloadCall_ProducesDefaultLatAndLon()
    {
        // Arrange
        decimal actualLat = 0;
        decimal actualLon = 0;
        _openWeatherServiceMock.OneCallAsync(
            Arg.Do<decimal>(x => actualLat = x),
            Arg.Do<decimal>(x => actualLon = x),
            Arg.Any<IEnumerable<Excludes>>(),
            Arg.Any<Units>())
            .Returns(Task.FromResult(GetSample(_defaultTemps)));

        // Act
        await _sut.GetForecastWeatherAnalysis();

        // Assert
        Assert.True(actualLat > 0);
        Assert.True(actualLon > 0);
    }

    [Fact]
    public async Task GetForecastWeatherAnalysis_NotInterestedInTodayWeather_WFStartsFromNextDay()
    {
        // Arrange
        const double nextDayTemp = 3.3;
        const double day5Temp = 7.7;
        var realWeatherTemps = new[] { 2, nextDayTemp, 4, 5.5, 6, day5Temp, 8, 9.9 };
        _openWeatherServiceMock.OneCallAsync(
            Arg.Any<decimal>(),
            Arg.Any<decimal>(),
            Arg.Any<IEnumerable<Excludes>>(),
            Arg.Any<Units>())
            .Returns(Task.FromResult(GetSample(realWeatherTemps)));

        // Act
        var wfs = await _sut.GetForecastWeatherAnalysis();

        // Assert
        Assert.Equal(3, wfs.First().TemperatureC);
    }

    [Fact]
    public async Task GetForecastWeatherAnalysis_5DaysForecastStartingNextDay_WF5ThDayIsRealWeather6ThDay()
    {
        // Arrange
        const double nextDayTemp = 3.3;
        const double day5Temp = 7.7;
        var realWeatherTemps = new[] { 2, nextDayTemp, 4, 5.5, 6, day5Temp, 8, 9.9 };
        _openWeatherServiceMock.OneCallAsync(
            Arg.Any<decimal>(),
            Arg.Any<decimal>(),
            Arg.Any<IEnumerable<Excludes>>(),
            Arg.Any<Units>())
            .Returns(Task.FromResult(GetSample(realWeatherTemps)));

        // Act
        var wfs = await _sut.GetForecastWeatherAnalysis();

        // Assert
        Assert.Equal(8, wfs.Last().TemperatureC);
    }

    [Fact]
    public async Task GetForecastWeatherAnalysis_ForecastingFor5DaysOnly_WFHas5Days()
    {
        // Arrange
        const double nextDayTemp = 3.3;
        const double day5Temp = 7.7;
        var realWeatherTemps = new[] { 2, nextDayTemp, 4, 5.5, 6, day5Temp, 8, 9.9 };
        _openWeatherServiceMock.OneCallAsync(
            Arg.Any<decimal>(),
            Arg.Any<decimal>(),
            Arg.Any<IEnumerable<Excludes>>(),
            Arg.Any<Units>())
            .Returns(Task.FromResult(GetSample(realWeatherTemps)));

        // Act
        var wfs = await _sut.GetForecastWeatherAnalysis();

        // Assert
        Assert.Equal(5, wfs.Count());
    }

    [Fact]
    public async Task GetForecastWeatherAnalysis_WFDoesntConsiderDecimal_RealWeatherTempRoundedProperly()
    {
        // Arrange
        const double nextDayTemp = 3.3;
        const double day5Temp = 7.7;
        var realWeatherTemps = new[] { 2, nextDayTemp, 4, 5.5, 6, day5Temp, 8, 9.9 };
        _openWeatherServiceMock.OneCallAsync(
            Arg.Any<decimal>(),
            Arg.Any<decimal>(),
            Arg.Any<IEnumerable<Excludes>>(),
            Arg.Any<Units>())
            .Returns(Task.FromResult(GetSample(realWeatherTemps)));

        // Act
        var wfs = await _sut.GetForecastWeatherAnalysis();

        // Assert
        Assert.Equal(3, wfs.First().TemperatureC);
        Assert.Equal(8, wfs.Last().TemperatureC);
    }

    [Fact]
    public async Task GetForecastWeatherAnalysis_TodayWeatherAnd6DaysForecastReceived_RealDateMatchesNextDay()
    {
        // Arrange
        const double nextDayTemp = 3.3;
        const double day5Temp = 7.7;
        var realWeatherTemps = new[] { 2, nextDayTemp, 4, 5.5, 6, day5Temp, 8, 9.9 };
        _openWeatherServiceMock.OneCallAsync(
            Arg.Any<decimal>(),
            Arg.Any<decimal>(),
            Arg.Any<IEnumerable<Excludes>>(),
            Arg.Any<Units>())
            .Returns(Task.FromResult(GetSample(realWeatherTemps)));

        // Act
        var wfs = await _sut.GetForecastWeatherAnalysis();

        // Assert
        Assert.Equal(new DateTime(2023, 1, 2), wfs.First().Date);
    }

    [Fact]
    public async Task GetForecastWeatherAnalysis_TodayWeatherAnd6DaysForecastReceived_RealDateMatchesLastDay()
    {
        // Arrange
        const double nextDayTemp = 3.3;
        const double day5Temp = 7.7;
        var realWeatherTemps = new[] { 2, nextDayTemp, 4, 5.5, 6, day5Temp, 8, 9.9 };
        _openWeatherServiceMock.OneCallAsync(
            Arg.Any<decimal>(),
            Arg.Any<decimal>(),
            Arg.Any<IEnumerable<Excludes>>(),
            Arg.Any<Units>())
            .Returns(Task.FromResult(GetSample(realWeatherTemps)));

        // Act
        var wfs = await _sut.GetForecastWeatherAnalysis();

        // Assert
        Assert.Equal(new DateTime(2023, 1, 6), wfs.Last().Date);
    }

    [Fact]
    public async Task GetForecastWeatherAnalysis_RequestsToOpenWeather_MetricUnitIsUsed()
    {
        // Arrange
        Units actualUnit = Units.Standard;
        _openWeatherServiceMock.OneCallAsync(
            Arg.Any<decimal>(),
            Arg.Any<decimal>(),
            Arg.Any<IEnumerable<Excludes>>(),
            Arg.Do<Units>(x => actualUnit = x))
            .Returns(Task.FromResult(GetSample(_defaultTemps)));

        // Act
        var _ = await _sut.GetForecastWeatherAnalysis();

        // Assert
        Assert.Equal(Units.Metric, actualUnit);
    }

    [Theory]
    [InlineData("Freezing", -1)]
    [InlineData("Freezing", 0)]
    [InlineData("Bracing", 1)]
    [InlineData("Bracing", 4.4)]
    [InlineData("Chilly", 5)]
    [InlineData("Chilly", 9.4)]
    [InlineData("Cool", 10)]
    [InlineData("Cool", 14.4)]
    [InlineData("Mild", 15)]
    [InlineData("Mild", 19.4)]
    [InlineData("Warm", 20)]
    [InlineData("Warm", 24.4)]
    [InlineData("Balmy", 25)]
    [InlineData("Balmy", 29.4)]
    [InlineData("Hot", 30)]
    [InlineData("Hot", 34.4)]
    [InlineData("Sweltering", 35)]
    [InlineData("Sweltering", 39.4)]
    [InlineData("Scorching", 40)]
    [InlineData("Scorching", 44.4)]
    [InlineData("Scorching", 46)]
    public async Task GetForecastWeatherAnalysis_Summary_MatchesTemp(string summary, double temp)
    {
        // Arrange
        var realWeatherTemps = new[] { 1, temp, 3, 4, 5, 6, 7, 8 };
        _openWeatherServiceMock.OneCallAsync(
            Arg.Any<decimal>(),
            Arg.Any<decimal>(),
            Arg.Any<IEnumerable<Excludes>>(),
            Arg.Any<Units>())
            .Returns(Task.FromResult(GetSample(realWeatherTemps)));

        // Act
        var wfs = await _sut.GetForecastWeatherAnalysis();

        // Assert
        Assert.Equal(summary, wfs.First().Summary);
    }

    private double[] _defaultTemps = new[] { -20d, -10d, 0d, 10d, 20d, 30d, 40d, 50d };
    private OneCallResponse GetSample(double[] temps)
    {
        var daily = new Daily[8];
        for(int i = 0; i < daily.Length; i++)
        {
            daily[i] = new Daily();
            daily[i].Dt = new DateTime(2023, 1, i + 1);
            daily[i].Temp = new Temp
            {
                Day = temps[i]
            };
        }
        return new OneCallResponse() { Daily = daily };
    }
}