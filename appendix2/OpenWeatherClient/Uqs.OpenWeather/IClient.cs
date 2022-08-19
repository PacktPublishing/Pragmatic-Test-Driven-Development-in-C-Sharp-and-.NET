
namespace Uqs.OpenWeather;

public interface IClient
{
    Task<OneCallResponse> OneCallAsync(decimal latitude, decimal longitude, IEnumerable<Excludes> excludes, Units unit);
}