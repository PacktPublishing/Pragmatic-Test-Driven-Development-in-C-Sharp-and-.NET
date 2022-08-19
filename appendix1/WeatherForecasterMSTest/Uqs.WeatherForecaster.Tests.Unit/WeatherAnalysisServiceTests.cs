using NSubstitute;
using Uqs.WeatherForecaster.Services;

namespace Uqs.WeatherForecaster.Tests.Unit;

[TestClass]
public class WeatherAnalysisServiceTests
{
    private IOpenWeatherService _openWeatherServiceMock = Substitute.For<IOpenWeatherService>();
    private WeatherAnalysisService _sut;
    private const decimal LAT = 2.2m;
    private const decimal LON = 1.1m;

    [TestInitialize]
    public void TestInitialize()
    {
        _sut = new(_openWeatherServiceMock);
    }

    [TestMethod]
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
        Assert.AreEqual(LAT, actualLat);
        Assert.AreEqual(LON, actualLon);
    }

    [TestMethod]
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
        Assert.IsTrue(actualLat > 0);
        Assert.IsTrue(actualLon > 0);
    }

    [TestMethod]
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
        Assert.AreEqual(3, wfs.First().TemperatureC);
    }

    [TestMethod]
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
        Assert.AreEqual(8, wfs.Last().TemperatureC);
    }

    [TestMethod]
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
        Assert.AreEqual(5, wfs.Count());
    }

    [TestMethod]
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
        Assert.AreEqual(3, wfs.First().TemperatureC);
        Assert.AreEqual(8, wfs.Last().TemperatureC);
    }

    [TestMethod]
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
        Assert.AreEqual(new DateTime(2023, 1, 2), wfs.First().Date);
    }

    [TestMethod]
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
        Assert.AreEqual(new DateTime(2023, 1, 6), wfs.Last().Date);
    }

    [TestMethod]
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
        Assert.AreEqual(Units.Metric, actualUnit);
    }

    [DataTestMethod]
    [DataRow("Freezing", -1)]
    [DataRow("Freezing", 0)]
    [DataRow("Bracing", 1)]
    [DataRow("Bracing", 4.4)]
    [DataRow("Chilly", 5)]
    [DataRow("Chilly", 9.4)]
    [DataRow("Cool", 10)]
    [DataRow("Cool", 14.4)]
    [DataRow("Mild", 15)]
    [DataRow("Mild", 19.4)]
    [DataRow("Warm", 20)]
    [DataRow("Warm", 24.4)]
    [DataRow("Balmy", 25)]
    [DataRow("Balmy", 29.4)]
    [DataRow("Hot", 30)]
    [DataRow("Hot", 34.4)]
    [DataRow("Sweltering", 35)]
    [DataRow("Sweltering", 39.4)]
    [DataRow("Scorching", 40)]
    [DataRow("Scorching", 44.4)]
    [DataRow("Scorching", 46)]
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
        Assert.AreEqual(summary, wfs.First().Summary);
    }

    private double[] _defaultTemps = new[] { -20d, -10d, 0d, 10d, 20d, 30d, 40d, 50d };
    private OneCallResponse GetSample(double[] temps)
    {
        var daily = new Daily[8];
        for (int i = 0; i < daily.Length; i++)
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