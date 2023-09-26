﻿using Claims.Core;
using Claims.Infrastructure;
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
                RuleFor(x => x.DamageCost).NotEmpty().GreaterThan(20);
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
            private readonly CosmosDbService _cosmosDbService;
            private readonly Auditer _auditer;

            public Handle(AuditContext auditContext, CosmosDbService cosmosDbService)
            {
                _auditer = new Auditer(auditContext);
                _cosmosDbService = cosmosDbService;
            }

            async Task<Response> IRequestHandler<Request, Response>.Handle(Request request, CancellationToken cancellationToken)
            {
                var claim = new Claim()
                {
                    CoverId = request.CoverId,
                    Name = request.Name,
                    Type = request.Type,
                    DamageCost = request.DamageCost,
                    Id = request.Id
                };

                await _cosmosDbService.AddItemAsync(claim);
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