using Claims.Core;
using Claims.Infrastructure;
using MediatR;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;

namespace Claims.Services.Covers
{
    public class GetAllCovers
    {
        public class Request : IRequest<Response>
        {
        }

        public class Response
        {
            public IEnumerable<CoverItem> Covers { get; set; }

            public Response()
            {
                Covers = new List<CoverItem>();
            }

            public class CoverItem
            {
                public string Id { get; set; }
                public DateOnly StartDate { get; set; }
                public DateOnly EndDate { get; set; }
                public CoverType Type { get; set; }
                public decimal Premium { get; set; }
            }
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
                var query = _container.GetItemQueryIterator<Cover>(new QueryDefinition("SELECT * FROM c"));

                var covers = new List<Response.CoverItem>();

                while (query.HasMoreResults)
                {
                    var response = await query.ReadNextAsync();

                    var tempList = response.Select(x => new Response.CoverItem()
                    {
                        Id = x.Id,
                        StartDate = x.StartDate,
                        EndDate = x.EndDate,
                        Premium = x.Premium,
                        Type = x.Type
                    }).ToList();

                    covers.AddRange(tempList);
                }

                return new Response()
                {
                    Covers = covers
                };
            }
        }
    }
}
