using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using Uqs.AppointmentBooking.Domain.DomainObjects;

namespace Uqs.AppointmentBooking.Domain.Repository;

public interface IEmployeeRepository : ICosmosRepository<Employee>
{
    public Task<IEnumerable<Employee>> GetAllAsync();
}

public class EmployeeRepository : CosmosRepository<Employee>, IEmployeeRepository
{
    public EmployeeRepository(CosmosClient cosmosClient, IOptions<ApplicationSettings> settings) :
        base(nameof(Customer), cosmosClient, settings)
    {
    }

    public Task<IEnumerable<Employee>> GetAllAsync()
    {
        return GetItemsAsync(new QueryDefinition("SELECT * FROM c"));
    }
}
