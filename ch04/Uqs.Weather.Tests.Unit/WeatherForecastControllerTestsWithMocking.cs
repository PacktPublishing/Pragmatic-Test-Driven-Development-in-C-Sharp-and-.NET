using Uqs.Weather.Controllers;
using Xunit;
using AdamTibi.OpenWeather;
using NSubstitute;

namespace Uqs.Weather.Tests.Unit;

public class WeatherForecastControllerTestsWithMocking
{
    [Fact]
    public async Task GetReal_NotInterestedInTodayWeather_WFStartsFromNextDay()
    {
        // Arrange
        const double NEXT_DAY_TEMP = 3.3;
        const double DAY5_TEMP = 7.7;
        var today = new DateTime(2022, 1, 1);
        var realWeatherTemps = new[] {2, NEXT_DAY_TEMP, 4, 5.5, 6, DAY5_TEMP, 8};
        var clientMock = Substitute.For<IClient>();
        clientMock.OneCallAsync(Arg.Any<decimal>(), Arg.Any<decimal>(), 
            Arg.Any<IEnumerable<Excludes>>(), Arg.Any<Units>())
            .Returns(_ => 
            {
                const int DAYS = 7;
                OneCallResponse res = new OneCallResponse();
                res.Daily = new Daily[DAYS];
                for (int i = 0; i < DAYS; i++)
                {
                    res.Daily[i] = new Daily();
                    res.Daily[i].Dt = today.AddDays(i);
                    res.Daily[i].Temp = new Temp();
                    res.Daily[i].Temp.Day = realWeatherTemps.ElementAt(i);
                }
                return Task.FromResult(res);
            });
        var controller = new WeatherForecastController(null!, clientMock, null!, null!);

        // Act
        IEnumerable<WeatherForecast> wfs = await controller.GetReal();

        // Assert
        Assert.Equal(3, wfs.First().TemperatureC);
    }

    [Fact]
    public async Task GetReal_RequestsToOpenWeather_MetricUnitIsUsed()
    {
        // Arrange
        var realWeatherTemps = new double[] { 1, 2, 3, 4, 5, 6, 7 };
        var today = default(DateTime);
        var clientMock = Substitute.For<IClient>();
        clientMock.OneCallAsync(Arg.Any<decimal>(), Arg.Any<decimal>(),
            Arg.Any<IEnumerable<Excludes>>(), Arg.Any<Units>())
            .Returns(_ =>
            {
                const int DAYS = 7;
                OneCallResponse res = new OneCallResponse();
                res.Daily = new Daily[DAYS];
                for (int i = 0; i < DAYS; i++)
                {
                    res.Daily[i] = new Daily();
                    res.Daily[i].Dt = today.AddDays(i);
                    res.Daily[i].Temp = new Temp();
                    res.Daily[i].Temp.Day = realWeatherTemps.ElementAt(i);
                }
                return Task.FromResult(res);
            });
        var controller = new WeatherForecastController(null!, clientMock, null!, null!);

        // Act
        var _ = await controller.GetReal();

        // Assert
        await clientMock.Received().OneCallAsync(Arg.Any<decimal>(), Arg.Any<decimal>(),
            Arg.Any<IEnumerable<Excludes>>(), Arg.Is<Units>(x => x == Units.Metric));
    }

}