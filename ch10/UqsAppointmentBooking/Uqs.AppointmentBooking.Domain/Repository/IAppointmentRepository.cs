using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using Uqs.AppointmentBooking.Domain.DomainObjects;

namespace Uqs.AppointmentBooking.Domain.Repository;

public interface IAppointmentRepository : ICosmosRepository<Appointment>
{
}

public class AppointmentRepository : CosmosRepository<Appointment>, IAppointmentRepository
{
    public AppointmentRepository(CosmosClient cosmosClient, IOptions<ApplicationSettings> settings) :
        base(nameof(Appointment), cosmosClient, settings)
    {
    }
}
