namespace Uqs.AppointmentBooking.Domain.Services;

public interface INowService
{
    DateTimeOffset Now { get; }
}

