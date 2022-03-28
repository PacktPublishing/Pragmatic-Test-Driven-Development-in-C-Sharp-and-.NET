using Microsoft.Extensions.Logging.Abstractions;
using Uqs.Weather.Controllers;
using Xunit;

namespace Uqs.Weather.Tests.Unit;

public class WeatherForecastControllerTests
{
    [Fact]
    public void ConvertCToF_0Celsius_32Fahrenheit()
    {
        const double EXPECTED = 32d;
        var logger = NullLogger<WeatherForecastController>.Instance;
        var controller = new WeatherForecastController(logger, null!, null!, null!);

        double actual = controller.ConvertCToF(0);

        Assert.Equal(EXPECTED, actual);
    }

    [Theory]
    [InlineData(-100, -148)]
    [InlineData(-10.1, 13.8)]
    [InlineData(10, 50)]

    public void ConvertCToF_Celsius_CorrectFahrenheit(double c, double f)
    {
        var logger = NullLogger<WeatherForecastController>.Instance;
        var controller = new WeatherForecastController(logger, 
            null!, null!, null!);
        double actual = controller.ConvertCToF(c);

        Assert.Equal(f, actual, 1);
    }
}