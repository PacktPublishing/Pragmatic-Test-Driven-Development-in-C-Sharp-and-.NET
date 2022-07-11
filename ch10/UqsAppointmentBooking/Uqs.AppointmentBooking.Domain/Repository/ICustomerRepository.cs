using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using Uqs.AppointmentBooking.Domain.DomainObjects;

namespace Uqs.AppointmentBooking.Domain.Repository;

public interface ICustomerRepository : ICosmosRepository<Customer>
{
    public Task<IEnumerable<Customer>> GetAll();
}

public class CustomerRepository : CosmosRepository<Customer>, ICustomerRepository
{
    public CustomerRepository(CosmosClient cosmosClient, IOptions<ApplicationSettings> settings) : 
        base(nameof(Customer), cosmosClient, settings)
    {
    }

    public Task<IEnumerable<Customer>> GetAll()
    {
        var queryDefinition = new QueryDefinition("SELECT * FROM c");
        
        return GetItemsAsync(queryDefinition);
    }
}
