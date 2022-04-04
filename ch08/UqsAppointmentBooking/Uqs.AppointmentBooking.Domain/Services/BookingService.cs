namespace Uqs.AppointmentBooking.Domain.Services;

public interface IBookingService
{
    public Task BookAppointment();
}

public class BookingService : IBookingService
{
    public Task BookAppointment()
    {
        throw new NotImplementedException();
    }
}
