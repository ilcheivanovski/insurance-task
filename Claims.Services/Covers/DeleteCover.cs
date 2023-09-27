using Claims.Core;
using Claims.Infrastructure.AuditContext;
using MediatR;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;

namespace Claims.Services.Covers
{
    public class DeleteCover
    {
        public class Request : IRequest<Response>
        {
            public string Id { get; set; }
        }

        public class Response
        {

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
                _auditer.AuditCover(request.Id, "DELETE");
                await _container.DeleteItemAsync<Cover>(request.Id, new(request.Id));

                return new Response();
            }
        }
    }
}
