using System.Net;

namespace Claims.Infrastructure.CosmosDb
{
    public interface ICosmosDbService
    {
        Task<HttpStatusCode> CreateDatabase(string ADatabaseName, CancellationToken ACancellationToken = default);

        Task<HttpStatusCode> CreateContainer(string ADatabaseName, string AContainerName, Guid AId, CancellationToken ACancellationToken = default);

        Task<HttpStatusCode> IsItemExists<T>(Guid AId, CancellationToken ACancellationToken = default) where T : class;

        Task<T> GetItem<T>(Guid AId, CancellationToken ACancellationToken = default) where T : class;

        Task<IEnumerable<T>> GetItems<T>(CancellationToken ACancellationToken = default) where T : class;

        Task<HttpStatusCode> AddItem<T>(Guid AId, T AItem, CancellationToken ACancellationToken = default);

        Task<HttpStatusCode> UpdateItem<T>(Guid AId, T AItem, CancellationToken ACancellationToken = default);

        Task<HttpStatusCode> DeleteItem<T>(Guid AId, CancellationToken ACancellationToken = default);
    }
}
