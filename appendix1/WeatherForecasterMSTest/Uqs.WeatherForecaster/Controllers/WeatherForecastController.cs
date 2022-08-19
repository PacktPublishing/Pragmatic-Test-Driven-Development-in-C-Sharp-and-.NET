using Microsoft.AspNetCore.Mvc;
using Uqs.WeatherForecaster.Services;

namespace Uqs.WeatherForecaster.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly IWeatherAnalysisService _weatherAnalysisService;

    public WeatherForecastController(IWeatherAnalysisService weatherAnalysisService)
    {
        _weatherAnalysisService = weatherAnalysisService;
    }

    [HttpGet]
    public async Task<IEnumerable<WeatherForecast>> GetReal([FromQuery] decimal? lat, [FromQuery] decimal? lon)
    {
        if (lat is null || lon is null)
        {
            return await _weatherAnalysisService.GetForecastWeatherAnalysis();
        }

        return await _weatherAnalysisService.GetForecastWeatherAnalysis(lat.Value, lon.Value);
    }
}