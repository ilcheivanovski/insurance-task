namespace Claims.Infrastructure.CosmosDb
{
    public interface ICosmosDbService
    {
        Task<IEnumerable<T>> GetAllItemsAsync<T>();
        Task<T> GetItemAsync<T>(string id);
        Task AddItemAsync<T>(T item, string id);
        Task DeleteItemAsync<T>(string id);
    }
}
