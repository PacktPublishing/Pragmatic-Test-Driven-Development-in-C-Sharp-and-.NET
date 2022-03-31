using Uqs.AppointmentBooking.Database.Domain;
using Uqs.AppointmentBooking.Domain.Report;

namespace Uqs.AppointmentBooking.Domain.Services;

public interface ISlotService
{
    Task<IEnumerable<Slots>> GetAvailableSlots(int? employeeId);
}

public class SlotsService : ISlotService
{
    private readonly ApplicationContext _context;

    internal const byte DAYS = 7;

    public SlotsService(ApplicationContext context)
    {
        _context = context;
    }

    public Task<IEnumerable<Slots>> GetAvailableSlots(int? employeeId)
    {
        throw new NotImplementedException();
    }
}