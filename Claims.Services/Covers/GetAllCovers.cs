using Claims.Core;
using Claims.Infrastructure.AuditContext;
using Claims.Infrastructure.CosmosDb;
using MediatR;
using Microsoft.AspNetCore.Components.Forms;
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
            private readonly ICosmosDbService _cosmosDbService;
            private readonly Auditer _auditer;

            public Handle(ICosmosDbService cosmosDbService, AuditContext auditContext)
            {
                _auditer = new Auditer(auditContext);
                _cosmosDbService = cosmosDbService;
            }


            async Task<Response> IRequestHandler<Request, Response>.Handle(Request request, CancellationToken cancellationToken)
            {
                var claims = await _cosmosDbService.GetAllItemsAsync<Cover>();

                var covers = claims.Select(x => new Response.CoverItem()
                {
                    Id = x.Id,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    Premium = x.Premium,
                    Type = x.Type
                }).ToList();

                return new Response()
                {
                    Covers = covers
                };
            }
        }
    }
}
