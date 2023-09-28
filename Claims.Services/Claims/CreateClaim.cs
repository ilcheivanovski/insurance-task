using Claims.Core;
using Claims.Infrastructure.AuditContext;
using Claims.Infrastructure.CosmosDb;
using FluentValidation;
using MediatR;

namespace Claims.Services.Claims
{
    public class CreateClaim
    {
        public class Request : IRequest<Response>
        {
            public string Id { get; set; }
            public string CoverId { get; set; }
            public DateTime Created { get; set; }
            public string Name { get; set; }
            public ClaimType Type { get; set; }
            public decimal DamageCost { get; set; }
        }

        public class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(x => x.DamageCost).NotEmpty();
            }
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
            private readonly Auditer _auditer;
            private readonly IValidator _validator;

            public Handle(AuditContext auditContext, ICosmosDbService cosmosDbService, IValidator<Request> validator)
            {
                _auditer = new Auditer(auditContext);
                _cosmosDbService = cosmosDbService;
                _validator = validator; // Injected validator
            }

            async Task<Response> IRequestHandler<Request, Response>.Handle(Request request, CancellationToken cancellationToken)
            {

                var validator = new Validator();
                var validationResult = await validator.ValidateAsync(request);
                var relatedCover = await _cosmosDbService.GetItemAsync<Cover>(request.CoverId);

                if (
                    validationResult.IsValid && relatedCover != null &&
                   (DateOnly.FromDateTime(request.Created) < relatedCover.StartDate ||
                    DateOnly.FromDateTime(request.Created) > relatedCover.EndDate)
                    )
                {
                    throw new FluentValidation.ValidationException(validationResult.Errors);
                }

                var claim = new Claim()
                {
                    CoverId = request.CoverId,
                    Name = request.Name,
                    Type = request.Type,
                    DamageCost = request.DamageCost,
                };

                await _cosmosDbService.AddItemAsync<Claim>(claim, claim.Id);
                _auditer.AuditClaim(claim.Id, "POST");


                return new Response()
                {
                    Id = claim.Id,
                    CoverId = claim.CoverId,
                    Created = claim.Created,
                    Name = claim.Name,
                    Type = claim.Type
                };
            }
        }
    }
}
