using Claims.Core;
using Claims.Infrastructure.CosmosDb;
using MediatR;
using Claim = Claims.Core.Claim;

namespace Claims.Services.Claims
{
    public class GetClaim
    {
        public class Request : IRequest<Response>
        {
            public string Id { get; set; }
        }

        public class Response
        {
            public string Id { get; set; }
            public string CoverId { get; set; }
            public DateTime Created { get; set; }
            public string Name { get; set; }
            public ClaimType Type { get; set; }
            public decimal DamageCost { get; set; }
        }

        public class Handle : IRequestHandler<Request, Response>
        {
            private readonly ICosmosDbService _cosmosDbService;

            public Handle(ICosmosDbService cosmosDbService)
            {
                _cosmosDbService = cosmosDbService;
            }

            async Task<Response> IRequestHandler<Request, Response>.Handle(Request request, CancellationToken cancellationToken)
            {
                var x = await _cosmosDbService.GetItemAsync<Claim>(request.Id);

                return new Response()
                {
                    Id = x.Id,
                    CoverId = x.CoverId,
                    Name = x.Name,
                    Type = x.Type,
                    DamageCost = x.DamageCost,
                    Created = x.Created
                };
            }
        }
    }
}
