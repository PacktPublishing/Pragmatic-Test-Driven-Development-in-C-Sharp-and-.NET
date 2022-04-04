using Uqs.AppointmentBooking.Domain.DomainObjects;
using Uqs.AppointmentBooking.Domain.Report;

namespace Uqs.AppointmentBooking.Domain.Services;

public interface ISlotsService
{
    Task<Slots> GetAvailableSlotsForEmployee(int serviceId, int employeeId);
}

public class SlotsService : ISlotsService
{

    public async Task<Slots> GetAvailableSlotsForEmployee(int serviceId, int employeeId)
    {
        throw new NotImplementedException();
    }

   
}