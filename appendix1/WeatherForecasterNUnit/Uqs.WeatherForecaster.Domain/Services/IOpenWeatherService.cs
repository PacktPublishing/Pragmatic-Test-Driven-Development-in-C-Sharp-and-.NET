using Uqs.WeatherForecaster.Domain.Dto;

namespace Uqs.WeatherForecaster.Domain.Services;

public interface IOpenWeatherService
{
    Task<OneCallResponse> OneCallAsync(decimal latitude, decimal longitude, IEnumerable<Excludes> excludes, Units unit);
}
