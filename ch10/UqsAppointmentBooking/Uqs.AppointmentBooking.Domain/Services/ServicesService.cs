using Uqs.AppointmentBooking.Domain.DomainObjects;
using Uqs.AppointmentBooking.Domain.Repository;

namespace Uqs.AppointmentBooking.Domain.Services;

public interface IServicesService
{
    Task<Service?> GetService(string id);

    Task<IEnumerable<Service>> GetActiveServices();
}

public class ServicesService : IServicesService
{
    private readonly IServiceRepository _serviceRepository;

    public ServicesService(IServiceRepository serviceRepository)
    {
        _serviceRepository = serviceRepository;
    }

    public async Task<IEnumerable<Service>> GetActiveServices()
        => await _serviceRepository.GetActiveServices();

    public async Task<Service?> GetService(string id)
        => await _serviceRepository.GetActiveService(id);
}