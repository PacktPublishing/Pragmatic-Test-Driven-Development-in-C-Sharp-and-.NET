using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using Uqs.AppointmentBooking.Domain.DomainObjects;

namespace Uqs.AppointmentBooking.Domain.Repository;

public interface IServiceRepository : ICosmosRepository<Service>
{
    Task<Service?> GetActiveService(string id);

    Task<IEnumerable<Service>> GetActiveServices();
}

public class ServiceRepository : CosmosRepository<Service>, IServiceRepository
{
    public ServiceRepository(CosmosClient cosmosClient, IOptions<ApplicationSettings> settings) :
        base(nameof(Service), cosmosClient, settings)
        {
        }

    public async Task<Service?> GetActiveService(string id)
    {
        var queryDefinition = new QueryDefinition(
            "SELECT * FROM c WHERE c.id = @id AND c.isActive = true")
                .WithParameter("@id", id);

        return (await GetItemsAsync(queryDefinition)).SingleOrDefault();
    }

    public Task<IEnumerable<Service>> GetActiveServices()
    {
        var queryDefinition = new QueryDefinition(
            "SELECT * FROM c WHERE c.isActive = true");

        return GetItemsAsync(queryDefinition);
    }
}