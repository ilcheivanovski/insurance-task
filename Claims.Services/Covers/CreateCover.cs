using Claims.Core;
using Claims.Infrastructure;
using MediatR;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;

namespace Claims.Services.Covers
{
    public class CreateCover
    {
        public class Request : IRequest<Response>
        {
            public DateOnly StartDate { get; set; }
            public DateOnly EndDate { get; set; }
            public CoverType Type { get; set; }
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
                _container = cosmosClient?.GetContainer("ClaimDb", "Cover")// GET COVER IN CONSTS
                     ?? throw new ArgumentNullException(nameof(cosmosClient));
            }

            async Task<Response> IRequestHandler<Request, Response>.Handle(Request request, CancellationToken cancellationToken)
            {
                var cover = new Cover()
                {
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    Type = request.Type
                };
                await _container.CreateItemAsync(cover, new PartitionKey(cover.Id));
                _auditer.AuditCover(cover.Id, "POST");

                return new Response()
                {
                    Id = cover.Id,
                    StartDate = cover.StartDate,
                    EndDate = cover.EndDate,
                    Premium = cover.Premium,
                    Type = cover.Type
                };
            }
        }
    }
}
