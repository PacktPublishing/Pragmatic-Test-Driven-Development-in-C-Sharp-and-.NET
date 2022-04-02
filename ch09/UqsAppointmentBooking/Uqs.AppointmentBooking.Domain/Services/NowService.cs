namespace Uqs.AppointmentBooking.Domain.Services;

public class NowService : INowService
{
    public DateTimeOffset Now => DateTimeOffset.Now;
}