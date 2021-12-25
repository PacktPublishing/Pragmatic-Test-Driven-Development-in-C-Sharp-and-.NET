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
    public async Task GetReal_IncomingRealWeather_MappedCorrectly()
    {
        var date = new DateTime(2022, 1, 1);
        var temps = new double[] {2.2, 3, 4.4, 5, 6.6, 7, 8.8};
        var client = new ClientStub(date, temps);
        var logger = NullLogger<WeatherForecastController>.Instance;
        var controller = new WeatherForecastController(logger, client, null!, null!);

        var weatherForecast = await controller.GetReal();

        Assert.Equal(expected, actual);
    }
}