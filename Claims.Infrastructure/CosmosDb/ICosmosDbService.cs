using Claims.Core;
using System.Net;

namespace Claims.Infrastructure.CosmosDb
{
    public interface ICosmosDbService
    {
        Task<IEnumerable<Claim>> GetClaimsAsync();
        Task<Claim> GetClaimAsync(string id);
        Task AddItemAsync(Claim item);
        //Task UpdateAsync(string id, Claim item);
        Task DeleteItemAsync(string id);
    }
}
