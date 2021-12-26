using AdamTibi.OpenWeather;

namespace Uqs.Weather.Tests.Unit.Stubs;

public class ClientStub : IClient
{
    private readonly DateTime _now;
    private readonly IEnumerable<double> _sevenDaysTemps;
    public Units? LastUnitSpy { get; set; }

    public ClientStub(DateTime now, IEnumerable<double> sevenDaysTemps)
    {
        _now = now;
        _sevenDaysTemps = sevenDaysTemps;
    }

    public Task<OneCallResponse> OneCallAsync(
        decimal latitude, decimal longitude, IEnumerable<Excludes> excludes, Units unit)
    {
        LastUnitSpy = unit;
        const int DAYS = 7;
        OneCallResponse res = new OneCallResponse();
        res.Daily = new Daily[DAYS];
        for (int i = 0; i < DAYS; i++)
        {
            res.Daily[i] = new Daily();
            res.Daily[i].Dt = _now.AddDays(i);
            res.Daily[i].Temp = new Temp();
            res.Daily[i].Temp.Day = _sevenDaysTemps.ElementAt(i);
        }
        return Task.FromResult(res);
    }
}