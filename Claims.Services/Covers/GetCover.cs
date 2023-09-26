using Claims.Core;
using Claims.Infrastructure;
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
            private readonly Container _container;
            private readonly Auditer _auditer;

            public Handle(CosmosClient cosmosClient, AuditContext auditContext)
            {
                _auditer = new Auditer(auditContext);
                _container = cosmosClient?.GetContainer("ClaimDb", "Cover")
                     ?? throw new ArgumentNullException(nameof(cosmosClient));
            }

            async Task<Response> IRequestHandler<Request, Response>.Handle(Request request, CancellationToken cancellationToken)
            {
                try
                {
                    var response = await _container.ReadItemAsync<Cover>(request.Id, new(request.Id));
                    var cover = response.Resource;

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
                    // TODO
                    //return NotFound();
                    throw new Exception();
                }
            }
        }
    }
}
