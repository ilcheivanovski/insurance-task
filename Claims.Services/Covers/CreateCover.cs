using Claims.Core;
using Claims.Infrastructure.AuditContext;
using FluentValidation;
using MediatR;
using Microsoft.Azure.Cosmos;

namespace Claims.Services.Covers
{
    public class CreateCover
    {
        public class Request : IRequest<Response>
        {
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public CoverType Type { get; set; }
            public decimal Premium { get; set; }

        }

        public class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(x => x.StartDate)
                    .Must(NotInPast)
                    .WithMessage("Date must be between now and one year from now.");

                RuleFor(x => new { x.StartDate, x.EndDate })
                              .NotEmpty() // Ensure StartDate is not empty
                              .Must(x => BeWithinOneYearRange(x.StartDate, x.EndDate)) // Custom validation method
                              .WithMessage("Start Date must be within one year of End Date");

            }
        }

        private static bool BeWithinOneYearRange(DateTime startDate, DateTime endDate)
        {
            // Calculate the one-year period from the StartDate
            DateTime oneYearFromStart = startDate.AddYears(1);

            // Check if EndDate is within the one-year period
            return endDate <= oneYearFromStart;
        }

        private static bool NotInPast(DateTime date)
        {
            // Get the current date as a DateOnly instance
            DateTime currentDate = DateTime.Today;

            // Check if the provided date is not in the past
            return date >= currentDate;
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
                _container = cosmosClient?.GetContainer("ClaimDb", "Cover") // GET COVER IN CONSTS
                     ?? throw new ArgumentNullException(nameof(cosmosClient));
            }

            async Task<Response> IRequestHandler<Request, Response>.Handle(Request request, CancellationToken cancellationToken)
            {
                var cover = new Cover()
                {
                    StartDate = DateOnly.FromDateTime(request.StartDate),
                    EndDate = DateOnly.FromDateTime(request.EndDate),
                    Type = request.Type,
                    Premium = request.Premium,
                };
                await _container.CreateItemAsync<Cover>(cover, new PartitionKey(cover.Id));
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
