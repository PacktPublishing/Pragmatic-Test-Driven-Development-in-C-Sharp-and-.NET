using System.Web;
using Newtonsoft.Json;

namespace Uqs.OpenWeather;

public class Client : IClient
{
    private readonly string _apiKey;
    private readonly HttpClient _httpClient;
    private const string BASE_URL = "https://api.openweathermap.org/data/2.5";

    public Client(string apiKey, HttpClient httpClient)
    {
        _apiKey = apiKey;
        _httpClient = httpClient;
    }

    public async Task<OneCallResponse> OneCallAsync(decimal latitude, decimal longitude, IEnumerable<Excludes> excludes, Units unit)
    {
        const string ONECALL_URL_TEMPLATE = "/onecall";
        var uriBuilder = new UriBuilder(BASE_URL + ONECALL_URL_TEMPLATE);
        var query = HttpUtility.ParseQueryString("");
        query["lat"] = latitude.ToString();
        query["lon"] = longitude.ToString();
        query["appid"] = _apiKey;
        if (excludes is { } && excludes.Any())
        {
            query["exclude"] = string.Join(',', excludes).ToLower();
        }
        query["units"] = unit.ToString().ToLower();

        uriBuilder.Query = query.ToString();

        var jsonResponse = await _httpClient.GetStringAsync(uriBuilder.Uri.AbsoluteUri);

        if (string.IsNullOrEmpty(jsonResponse))
        {
            throw new InvalidOperationException("No response from the service");
        }

        OneCallResponse? oneCallResponse = JsonConvert.DeserializeObject<OneCallResponse>(jsonResponse);

        if (oneCallResponse is null)
        {
            // This will never be hit
            throw new Exception();
        }

        return oneCallResponse;
    }

}