using Microsoft.Azure.Cosmos;

namespace Uqs.AppointmentBooking.Domain.Repository;

public interface ICosmosRepository<T> where T : IEntity
{
    Task<IEnumerable<T>> GetItemsAsync(QueryDefinition queryDefinition);
    Task<T?> GetItemAsync(string id);
    Task AddItemAsync(T item);
    Task UpdateItemAsync(string id, T item);
    Task DeleteItemAsync(string id);
}
