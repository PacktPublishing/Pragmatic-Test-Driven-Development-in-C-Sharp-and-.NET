namespace Uqs.Weather.Wrappers;

public class NowWrapper : INowWrapper
{
    public DateTime Now => DateTime.Now;
}