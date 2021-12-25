using Microsoft.Extensions.Logging.Abstractions;
using Uqs.Weather.Controllers;
using Uqs.Weather.Tests.Unit.Stubs;
using Xunit;

namespace Uqs.Weather.Tests.Unit;

public class WeatherForecastControllerTests
{
    [Fact]
    public void ConvertCToF_0Celsius_32Fahrenheit()
    {
        const double expected = 32d;
        var logger = NullLogger<WeatherForecastController>.Instance;
        var controller = new WeatherForecastController(logger, null!, null!, null!);

        double actual = controller.ConvertCToF(0);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(-100, -148)]
    [InlineData(-10.1, 13.8)]
    [InlineData(10, 50)]

    public void ConvertCToF_Celcius_CorrectFahrenheit(double c, double f)
    {
        var logger = NullLogger<WeatherForecastController>.Instance;
        var controller = new WeatherForecastController(logger, 
            null!, null!, null!);
        double actual = controller.ConvertCToF(c);

        Assert.Equal(f, actual, 1);
    }

    [Fact]
    public async Task GetReal_NotInterestedInTodayWeather_WFStartsFromNextDay()
    {
        // Arrange
        const double nextDayTemp = 3.3;
        const double day5Temp = 7.7;
        var today = new DateTime(2022, 1, 1);
        var realWeatherTemps = new double[] {2, nextDayTemp, 4, 5.5, 6, day5Temp, 8};
        var clientStub = new ClientStub(today, realWeatherTemps);
        var logger = NullLogger<WeatherForecastController>.Instance;
        var controller = new WeatherForecastController(logger, clientStub, null!, null!);

        // Act
        IEnumerable<WeatherForecast> wfs = await controller.GetReal();

        // Assert
        Assert.Equal(3, wfs.First().TemperatureC);
    }

    [Fact]
    public async Task GetReal_5DaysForecastStartingNextDay_WF5ThDayIsRealWeather6ThDay()
    {
        const double nextDayTemp = 3.3;
        const double day5Temp = 7.7;
        var today = new DateTime(2022, 1, 1);
        var realWeatherTemps = new double[] { 2, nextDayTemp, 4, 5.5, 6, day5Temp, 8 };
        var clientStub = new ClientStub(today, realWeatherTemps);
        var logger = NullLogger<WeatherForecastController>.Instance;
        var controller = new WeatherForecastController(logger, clientStub, null!, null!);

        IEnumerable<WeatherForecast> wfs = await controller.GetReal();

        Assert.Equal(8, wfs.Last().TemperatureC);
    }

    [Fact]
    public async Task GetReal_ForecastingFor5DaysOnly_WFHas5Days()
    {
        const double nextDayTemp = 3.3;
        const double day5Temp = 7.7;
        var today = new DateTime(2022, 1, 1);
        var realWeatherTemps = new double[] { 2, nextDayTemp, 4, 5.5, 6, day5Temp, 8 };
        var clientStub = new ClientStub(today, realWeatherTemps);
        var logger = NullLogger<WeatherForecastController>.Instance;
        var controller = new WeatherForecastController(logger, clientStub, null!, null!);

        IEnumerable<WeatherForecast> wfs = await controller.GetReal();

        Assert.Equal(5, wfs.Count());
    }

    [Fact]
    public async Task GetReal_WFDoesntConsiderDecimal_RealWeatherTempRoundedProperly()
    {
        const double nextDayTemp = 3.3;
        const double day5Temp = 7.7;
        var today = new DateTime(2022, 1, 1);
        var realWeatherTemps = new double[] { 2, nextDayTemp, 4, 5.5, 6, day5Temp, 8 };
        var clientStub = new ClientStub(today, realWeatherTemps);
        var logger = NullLogger<WeatherForecastController>.Instance;
        var controller = new WeatherForecastController(logger, clientStub, null!, null!);

        IEnumerable<WeatherForecast> wfs = await controller.GetReal();

        Assert.Equal(3, wfs.First().TemperatureC);
        Assert.Equal(8, wfs.Last().TemperatureC);
    }

    [Fact]
    public async Task GetReal_TodayWeatherAnd6DaysForecastReceived_RealDateMatchesNextDay()
    {
        const double nextDayTemp = 3.3;
        const double day5Temp = 7.7;
        var today = new DateTime(2022, 1, 1);
        var realWeatherTemps = new double[] { 2, nextDayTemp, 4, 5.5, 6, day5Temp, 8 };
        var clientStub = new ClientStub(today, realWeatherTemps);
        var logger = NullLogger<WeatherForecastController>.Instance;
        var controller = new WeatherForecastController(logger, clientStub, null!, null!);

        IEnumerable<WeatherForecast> wfs = await controller.GetReal();

        Assert.Equal(new DateTime(2022, 1, 2), wfs.First().Date);
    }

    [Fact]
    public async Task GetReal_TodayWeatherAnd6DaysForecastReceived_RealDateMatchesLastDay()
    {
        const double nextDayTemp = 3.3;
        const double day5Temp = 7.7;
        var today = new DateTime(2022, 1, 1);
        var realWeatherTemps = new double[] { 2, nextDayTemp, 4, 5.5, 6, day5Temp, 8 };
        var clientStub = new ClientStub(today, realWeatherTemps);
        var logger = NullLogger<WeatherForecastController>.Instance;
        var controller = new WeatherForecastController(logger, clientStub, null!, null!);

        IEnumerable<WeatherForecast> wfs = await controller.GetReal();

        Assert.Equal(new DateTime(2022, 1, 6), wfs.Last().Date);
    }
}