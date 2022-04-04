using Uqs.AppointmentBooking.Domain.DomainObjects;

namespace Uqs.AppointmentBooking.Domain.Services;

public interface IServicesService
{
    Task<Service?> GetService(int id);

    Task<IEnumerable<Service>> GetActiveServices();
}

public class ServicesService : IServicesService
{
    public ServicesService()
    {

    }

    public async Task<IEnumerable<Service>> GetActiveServices()
        => throw new NotImplementedException();

    public async Task<Service?> GetService(int id)
        => throw new NotImplementedException();
}