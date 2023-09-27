using Claims.Core;
using Claims.Infrastructure.CosmosDb;
using MediatR;

namespace Claims.Services.Claims
{
    public class GetAllClaims
    {
        public class Request : IRequest<Response>
        {
        }

        public class Response
        {
            public IEnumerable<ClaimItem> Claims { get; set; }

            public Response()
            {
                Claims = new List<ClaimItem>();
            }

            public class ClaimItem
            {
                public string Id { get; set; }
                public string CoverId { get; set; }
                public DateTime Created { get; set; }
                public string Name { get; set; }
                public ClaimType Type { get; set; }
                public decimal DamageCost { get; set; }
            }
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
                var claims = await _cosmosDbService.GetAllItemsAsync<Claim>();

                var covers = claims.Select(x => new Response.ClaimItem()
                {
                    Id = x.Id,
                    CoverId = x.CoverId,
                    Name = x.Name,
                    Type = x.Type,
                    DamageCost = x.DamageCost,
                    Created = x.Created
                }).ToList();

                return new Response()
                {
                    Claims = covers
                };
            }
        }
    }
}
