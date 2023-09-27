using Claims.Services.Claims;
using Claims.Services.Covers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClaimsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ClaimsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public Task<GetAllClaims.Response> GetAllClaims()
        {
            return _mediator.Send(new GetAllClaims.Request());
        }

        [HttpPost]
        public async Task<CreateClaim.Response> CreateClaim([FromBody] CreateClaim.Request request)
        {
            return await _mediator.Send(request);
        }

        [HttpDelete("{id}")]
        public Task DeleteAsync(string id)
        {
            return _mediator.Send(new DeleteClaim.Request() { Id = id });
        }

        [HttpGet("{id}")]
        public Task<GetClaim.Response> GetClaim(string id)
        {
            return _mediator.Send(new GetClaim.Request() { Id = id });
        }
    }
}