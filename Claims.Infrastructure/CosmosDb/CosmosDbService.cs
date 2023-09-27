using System.Net;
using Microsoft.Azure.Cosmos;
using Claims.Infrastructure.Settings;
using Claims.Core;
using System.Net.Sockets;

namespace Claims.Infrastructure.CosmosDb
{
    public class CosmosDbService : ICosmosDbService
    {
        private Container? _container { get; set; }

        private CosmosClient _cosmosClient { get; set; }

        private string _databaseName { get; set; }

        public CosmosDbService(CosmosDbSettings AConfiguration, CosmosClient cosmosClient)
        {
            var LAccount = AConfiguration.Account;
            var LKey = AConfiguration.Key;

            _databaseName = AConfiguration.DatabaseName;
            _cosmosClient = cosmosClient;
            _container = null;
        }

        private void InitContainer<T>()
        {
            // Names of CosmosDb container and item model used in the container must be the same
            var LModelName = typeof(T).Name;
            _container = _cosmosClient.GetContainer(_databaseName, LModelName);
        }

        public async Task<IEnumerable<T>> GetAllItemsAsync<T>()
        {
            if (_container == null) InitContainer<T>();
            var query = _container.GetItemQueryIterator<T>(new QueryDefinition("SELECT * FROM c"));
            var results = new List<T>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();

                results.AddRange(response.ToList());
            }
            return results;
        }

        public async Task<T> GetItemAsync<T>(string id)
        {
            try
            {
                if (_container == null) InitContainer<T>();
                var response = await _container.ReadItemAsync<T>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return default;
            }
        }


        public Task AddItemAsync<T>(T item, string id)
        {
            if (_container == null) InitContainer<T>();
            return _container.CreateItemAsync(item, new PartitionKey(id));
        }

        public Task DeleteItemAsync<T>(string id)
        {
            if (_container == null) InitContainer<T>();
            return _container.DeleteItemAsync<Claim>(id, new PartitionKey(id));
        }
    }
}