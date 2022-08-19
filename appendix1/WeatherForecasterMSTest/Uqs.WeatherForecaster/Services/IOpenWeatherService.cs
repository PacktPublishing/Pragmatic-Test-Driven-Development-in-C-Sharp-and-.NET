namespace Uqs.WeatherForecaster.Services;

public interface IOpenWeatherService
{
    Task<OneCallResponse> OneCallAsync(decimal latitude, decimal longitude, IEnumerable<Excludes> excludes, Units unit);
}
