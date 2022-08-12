namespace Uqs.WeatherForecaster.Services;

public interface IWeatherAnalysisService
{
    Task<IEnumerable<WeatherForecast>> GetForecastWeatherAnalysis();
    Task<IEnumerable<WeatherForecast>> GetForecastWeatherAnalysis(decimal lat, decimal lon);
}
