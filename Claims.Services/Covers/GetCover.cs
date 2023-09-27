using Claims.Core;
using Claims.Infrastructure;
using Claims.Infrastructure.CosmosDb;
using MediatR;
using Microsoft.Azure.Cosmos;

namespace Claims.Services.Covers
{
    public class GetCover
    {
        public class Request : IRequest<Response>
        {
            public string Id { get; set; }
        }

        public class Response
        {
            public string Id { get; set; }
            public DateOnly StartDate { get; set; }
            public DateOnly EndDate { get; set; }
            public CoverType Type { get; set; }
            public decimal Premium { get; set; }
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
                try
                {
                    var cover = await _cosmosDbService.GetItemAsync<Cover>(request.Id);

                    return new Response()
                    {
                        Id = cover.Id,
                        StartDate = cover.StartDate,
                        EndDate = cover.EndDate,
                        Premium = cover.Premium,
                        Type = cover.Type
                    };
                }
                catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
            }
        }
    }
}
