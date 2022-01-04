using Uqs.Weather.Controllers;
using Xunit;
using AdamTibi.OpenWeather;
using NSubstitute;
using Microsoft.Extensions.Logging;
using Uqs.Weather.Wrappers;

namespace Uqs.Weather.Tests.Unit;

public class WeatherForecastTestsReadable
{
    private readonly ILogger<WeatherForecastController> _loggerMock = Substitute.For<ILogger<WeatherForecastController>>();
    private readonly INowWrapper _nowWrapperMock = Substitute.For<INowWrapper>();
    private readonly IRandomWrapper _randomWrapperMock = Substitute.For<IRandomWrapper>();
    private readonly IClient _clientMock = Substitute.For<IClient>();
    private readonly WeatherForecastController _sut;

    public WeatherForecastTestsReadable()
    {
        _sut = new WeatherForecastController(_loggerMock, _clientMock, _nowWrapperMock, _randomWrapperMock);
    }

    [Fact]
    public async Task GetReal_NotInterestedInTodayWeather_WFStartsFromNextDay()
    {
        // Arrange
        OneCallResponse res = new OneCallResponseBuilder()
            .SetTemps(new []{ 0, 3.3, 0, 0, 0, 0, 0 })
            .Build();

        _clientMock.OneCallAsync(Arg.Any<decimal>(), Arg.Any<decimal>(),
            Arg.Any<IEnumerable<Excludes>>(), Arg.Any<Units>())
            .Returns(res);

        // Act
        IEnumerable<WeatherForecast> wfs = await _sut.GetReal();

        // Assert
        Assert.Equal(3, wfs.First().TemperatureC);
    }

    [Fact]
    public async Task GetReal_RequestsToOpenWeather_MetricUnitIsUsed()
    {
        // Arrange
        OneCallResponse res = new OneCallResponseBuilder().Build();
        _clientMock.OneCallAsync(Arg.Any<decimal>(), Arg.Any<decimal>(),
            Arg.Any<IEnumerable<Excludes>>(), Arg.Any<Units>())
            .Returns(res);

        // Act
        IEnumerable<WeatherForecast> wfs = await _sut.GetReal();

        // Assert
        await _clientMock.Received().OneCallAsync(Arg.Any<decimal>(), Arg.Any<decimal>(),
            Arg.Any<IEnumerable<Excludes>>(), Arg.Is<Units>(x => x == Units.Metric));
    }

    public class OneCallResponseBuilder
    {
        private int _days = 7;
        private DateTime _today = new (2022, 1, 1);
        private double[] _temps = new[] { 2, 3.3, 4, 5.5, 6, 7.7, 8 };

        public OneCallResponseBuilder SetDays(int days)
        {
            _days = days;
            return this;
        }

        public OneCallResponseBuilder SetToday(DateTime today)
        {
            _today = today;
            return this;
        }

        public OneCallResponseBuilder SetTemps(double[] temps)
        {
            _temps = temps;
            return this;
        }

        public OneCallResponse Build()
        {
            var res = new OneCallResponse();
            res.Daily = new Daily[_days];
            for (int i = 0; i < _days; i++)
            {
                res.Daily[i] = new Daily();
                res.Daily[i].Dt = _today.AddDays(i);
                res.Daily[i].Temp = new Temp();
                res.Daily[i].Temp.Day = _temps.ElementAt(i);
            }
            return res;
        }
    }



}