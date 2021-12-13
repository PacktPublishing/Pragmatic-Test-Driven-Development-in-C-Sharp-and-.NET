using AdamTibi.OpenWeather;

namespace Uqs.Weather;

public class ClientStub : IClient
{
    public Task<OneCallResponse> OneCallAsync(
        decimal latitude, decimal longitude, IEnumerable<Excludes> excludes, Units unit)
    {
        const int DAYS = 7;
        OneCallResponse res = new OneCallResponse();
        res.Daily = new Daily[DAYS];
        DateTime now = DateTime.Now;
        for (int i = 0; i < DAYS; i++)
        {
            res.Daily[i] = new Daily();
            res.Daily[i].Dt = now.AddDays(i);
            res.Daily[i].Temp = new Temp();
            res.Daily[i].Temp.Day = Random.Shared.Next(-20, 55);
        }
        return Task.FromResult(res);
    }
}