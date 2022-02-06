using Microsoft.AspNetCore.Mvc;
using AdamTibi.OpenWeather;
using Uqs.Weather.Wrappers;

namespace Uqs.Weather.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private const int FORECAST_DAYS = 5;
    private readonly IClient _client;
    private readonly INowWrapper _nowWrapper;
    private readonly IRandomWrapper _randomWrapper;
    private readonly ILogger<WeatherForecastController> _logger;

    private static readonly string[] _summaries = {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild",
        "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    public WeatherForecastController(ILogger<WeatherForecastController> logger,
        IClient client, INowWrapper nowWrapper, IRandomWrapper randomWrapper)
    {
        _logger = logger;
        _client = client;
        _nowWrapper = nowWrapper;
        _randomWrapper = randomWrapper;
    }

    [HttpGet("ConvertCToF")]
    public double ConvertCToF(double c)
    {
        double f = c * (9d / 5d) + 32;
        _logger.LogInformation("conversion requested");
        return f;
    }

    [HttpGet("GetRealWeatherForecast")]
    public async Task<IEnumerable<WeatherForecast>> GetReal()
    {
        const decimal GREENWICH_LAT = 51.4810m;
        const decimal GREENWICH_LON = 0.0052m;
        OneCallResponse res = await _client.OneCallAsync
            (GREENWICH_LAT, GREENWICH_LON, new[] {
                Excludes.Current, Excludes.Minutely,
                Excludes.Hourly, Excludes.Alerts }, Units.Metric);

        WeatherForecast[] wfs = new WeatherForecast[FORECAST_DAYS];
        for (int i = 0; i < wfs.Length; i++)
        {
            var wf = wfs[i] = new WeatherForecast();
            wf.Date = res.Daily[i + 1].Dt;
            double forecastedTemp = res.Daily[i + 1].Temp.Day;
            wf.TemperatureC = (int)Math.Round(forecastedTemp);
            wf.Summary = MapFeelToTemp(wf.TemperatureC);
        }
        return wfs;
    }

    private string MapFeelToTemp(int temperatureC)
    {
        if (temperatureC <= 0)
        {
            return _summaries.First();
        }
        int summariesIndex = (temperatureC / 5) + 1;
        if (summariesIndex >= _summaries.Length)
        {
            return _summaries.Last();
        }
        return _summaries[summariesIndex];
    }

    [HttpGet("GetRandomWeatherForecast")]
    public IEnumerable<WeatherForecast> GetRandom()
    {
        WeatherForecast[] wfs = new WeatherForecast[FORECAST_DAYS];
        for (int i = 0; i < wfs.Length; i++)
        {
            var wf = wfs[i] = new WeatherForecast();
            wf.Date = _nowWrapper.Now.AddDays(i + 1);
            wf.TemperatureC = _randomWrapper.Next(-20, 55);
            wf.Summary = MapFeelToTemp(wf.TemperatureC);
        }
        return wfs;
    }
}