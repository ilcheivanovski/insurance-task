using Claims.Core;
using Claims.Infrastructure;
using Claims.Infrastructure.CosmosDb;
using MediatR;

namespace Claims.Services.Covers
{
    public class DeleteClaim
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
            private readonly CosmosDbService _cosmosDbService;
            private readonly Auditer _auditer;

            public Handle(AuditContext auditContext, CosmosDbService cosmosDbService)
            {
                _auditer = new Auditer(auditContext);
                _cosmosDbService = cosmosDbService;
            }

            async Task<Response> IRequestHandler<Request, Response>.Handle(Request request, CancellationToken cancellationToken)
            {
                _auditer.AuditClaim(request.Id, "DELETE");
                await _cosmosDbService.DeleteItemAsync(request.Id);

                return new Response();
            }
        }
    }
}
