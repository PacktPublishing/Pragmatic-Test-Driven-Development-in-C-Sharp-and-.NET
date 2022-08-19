namespace Uqs.WeatherForecaster.Services;

public class WeatherAnalysisService : IWeatherAnalysisService
{
    private const decimal GREENWICH_LAT = 51.4810m;
    private const decimal GREENWICH_LON = 0.0052m;
    private const int FORECAST_DAYS = 5;
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly IOpenWeatherService _openWeatherService;

	public WeatherAnalysisService(IOpenWeatherService openWeatherService)
	{
        _openWeatherService = openWeatherService;
	}

    public Task<IEnumerable<WeatherForecast>> GetForecastWeatherAnalysis() => 
        GetForecastWeatherAnalysis(GREENWICH_LAT, GREENWICH_LON);

    public async Task<IEnumerable<WeatherForecast>> GetForecastWeatherAnalysis(decimal lat, decimal lon)
	{
        OneCallResponse res = await _openWeatherService.OneCallAsync
        (lat, lon, new[] {
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
