using Uqs.AppointmentBooking.Domain.DomainObjects;
using Uqs.AppointmentBooking.Domain.Report;

namespace Uqs.AppointmentBooking.Domain.Services;

public interface ISlotsService
{
    Task<Slots> GetAvailableSlotsForEmployee(int serviceId, int employeeId);

    Task<Slots> GetCombinedAvailableSlots(int serviceId);
}

public class SlotsService : ISlotsService
{
    public async Task<Slots> GetAvailableSlotsForEmployee(int serviceId, int employeeId)
    {
        throw new NotImplementedException();
    }

    public async Task<Slots> GetCombinedAvailableSlots(int serviceId)
    {
        throw new NotImplementedException();
    }
}