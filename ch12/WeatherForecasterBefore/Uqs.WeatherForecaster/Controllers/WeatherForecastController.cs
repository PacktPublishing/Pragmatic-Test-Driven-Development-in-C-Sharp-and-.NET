using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web;

namespace Uqs.WeatherForecaster.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private const decimal GREENWICH_LAT = 51.4810m;
    private const decimal GREENWICH_LON = 0.0052m;

    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    [HttpGet]
    public async Task<IEnumerable<WeatherForecast>> GetReal(
        [FromQuery] decimal lat = GREENWICH_LAT,
        [FromQuery] decimal lon = GREENWICH_LON)
    {
        var res = (await OneCallAsync(lat, lon)).ToArray();

        WeatherForecast[] wfs = new WeatherForecast[5];
        for (int i = 0; i < wfs.Length; i++)
        {
            var wf = wfs[i] = new WeatherForecast();
            wf.Date = res[i].Item1;
            double forecastedTemp = (double)res[i].Item2;
            wf.TemperatureC = (int)Math.Round(forecastedTemp);
            wf.Summary = MapFeelToTemp(wf.TemperatureC);
        }
        return wfs;
    }

    private static async Task<IEnumerable<(DateTime,decimal)>> OneCallAsync(decimal latitude, decimal longitude)
    {
        var uriBuilder = new UriBuilder("https://api.openweathermap.org/data/2.5/onecall");
        var query = HttpUtility.ParseQueryString("");
        query["lat"] = latitude.ToString();
        query["lon"] = longitude.ToString();
        query["appid"] = "[FillWithYours]";
        query["units"] = "metric";
        query["exclude"] = "current,minutely,hourly,alerts";

        uriBuilder.Query = query.ToString();
        var httpClient = new HttpClient();
        var jsonResponse = await httpClient.GetStringAsync(uriBuilder.Uri.AbsoluteUri);

        if (string.IsNullOrEmpty(jsonResponse))
        {
            throw new InvalidOperationException("No response from the service");
        }

        var res = JsonConvert.DeserializeObject<JObject>(jsonResponse)!;

        (DateTime, decimal)[] wfs = new (DateTime, decimal)[5];
        for (int i = 0; i < wfs.Length; i++)
        {
            wfs[i] = new(
                DateTimeOffset.FromUnixTimeSeconds((long)res["daily"][i + 1]["dt"]).DateTime, 
                (decimal)res["daily"][i + 1]["temp"]["day"]);
        }

        return wfs;
    }

    private static string MapFeelToTemp(int temperatureC)
    {
        if (temperatureC <= 0)
        {
            return Summaries.First();
        }
        int summariesIndex = (temperatureC / 5) + 1;
        if (summariesIndex >= Summaries.Length)
        {
            return Summaries.Last();
        }
        return Summaries[summariesIndex];
    }
}