using Microsoft.EntityFrameworkCore;
using Uqs.AppointmentBooking.Domain.Database;
using Uqs.AppointmentBooking.Domain.DomainObjects;

namespace Uqs.AppointmentBooking.Domain.Services;

public interface IServicesService
{
    Task<Service?> GetService(int id);

    Task<IEnumerable<Service>> GetActiveServices();
}

public class ServicesService : IServicesService
{
    private readonly ApplicationContext _context;

    public ServicesService(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Service>> GetActiveServices()
        => await _context.Services!.Where(x => x.IsActive).ToArrayAsync();

    public async Task<Service?> GetService(int id)
        => await _context.Services!.SingleOrDefaultAsync(x => x.IsActive && x.Id == id);
}