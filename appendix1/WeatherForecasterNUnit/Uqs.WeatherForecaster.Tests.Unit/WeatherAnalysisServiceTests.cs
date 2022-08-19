using NSubstitute;
using Uqs.WeatherForecaster.Services;

namespace Uqs.WeatherForecaster.Tests.Unit;

public class WeatherAnalysisServiceTests
{
    private IOpenWeatherService _openWeatherServiceMock = Substitute.For<IOpenWeatherService>();
    private WeatherAnalysisService _sut;
    private const decimal LAT = 2.2m;
    private const decimal LON = 1.1m;

    [SetUp]
    public void Setup()
    {
        _sut = new(_openWeatherServiceMock);
    }

    [Test]
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
        Assert.That(actualLat, Is.EqualTo(LAT));
        Assert.That(actualLon, Is.EqualTo(LON));
    }

    [Test]
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

    [Test]
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
        Assert.That(wfs.First().TemperatureC, Is.EqualTo(3));
    }

    [Test]
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
        Assert.That(wfs.Last().TemperatureC, Is.EqualTo(8));
    }

    [Test]
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
        Assert.That(wfs.Count(), Is.EqualTo(5));
    }

    [Test]
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
        Assert.That(wfs.First().TemperatureC, Is.EqualTo(3));
        Assert.That(wfs.Last().TemperatureC, Is.EqualTo(8));
    }

    [Test]
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
        Assert.That(wfs.First().Date, Is.EqualTo(new DateTime(2023, 1, 2)));
    }

    [Test]
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
        Assert.That(wfs.Last().Date, Is.EqualTo(new DateTime(2023, 1, 6)));
    }

    [Test]
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
        Assert.That(actualUnit, Is.EqualTo(Units.Metric));
    }

    [Theory]
    [TestCase("Freezing", -1)]
    [TestCase("Freezing", 0)]
    [TestCase("Bracing", 1)]
    [TestCase("Bracing", 4.4)]
    [TestCase("Chilly", 5)]
    [TestCase("Chilly", 9.4)]
    [TestCase("Cool", 10)]
    [TestCase("Cool", 14.4)]
    [TestCase("Mild", 15)]
    [TestCase("Mild", 19.4)]
    [TestCase("Warm", 20)]
    [TestCase("Warm", 24.4)]
    [TestCase("Balmy", 25)]
    [TestCase("Balmy", 29.4)]
    [TestCase("Hot", 30)]
    [TestCase("Hot", 34.4)]
    [TestCase("Sweltering", 35)]
    [TestCase("Sweltering", 39.4)]
    [TestCase("Scorching", 40)]
    [TestCase("Scorching", 44.4)]
    [TestCase("Scorching", 46)]
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
        Assert.That(wfs.First().Summary, Is.EqualTo(summary));
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