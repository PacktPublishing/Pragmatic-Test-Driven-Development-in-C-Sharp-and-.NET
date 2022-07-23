using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using System.Net;

namespace Uqs.AppointmentBooking.Domain.Repository;

public abstract class CosmosRepository<T> : ICosmosRepository<T> where T : IEntity
{
    protected CosmosClient CosmosClient { get; }
    protected Microsoft.Azure.Cosmos.Database Database { get; }
    protected Container Container { get; }

    public CosmosRepository(string containerId, CosmosClient cosmosClient, IOptions<ApplicationSettings> settings)
    {
        CosmosClient = cosmosClient;
        Database = cosmosClient.GetDatabase(settings.Value.DatabaseId);
        Container = Database.GetContainer(containerId);
    }

    public Task AddItemAsync(T item)
    {
        return Container.CreateItemAsync(item, new PartitionKey(item.Id));
    }

    public Task DeleteItemAsync(string id)
    {
        return Container.DeleteItemAsync<T>(id, new PartitionKey(id));
    }

    public async Task<T?> GetItemAsync(string id)
    {
        try
        {
            ItemResponse<T> response = await Container.ReadItemAsync<T>(id, new PartitionKey(id));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return default;
        }
    }

    public async Task<IEnumerable<T>> GetItemsAsync(QueryDefinition queryDefinition)
    {
        var query = Container.GetItemQueryIterator<T>(queryDefinition);
        List<T> results = new List<T>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response.ToList());
        }

        return results;
    }

    public async Task UpdateItemAsync(string id, T item)
    {
        await Container.UpsertItemAsync(item, new PartitionKey(id));
    }
}