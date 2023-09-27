using Claims.Services.Covers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Controllers;

[ApiController]
[Route("[controller]")]
public class CoversController : ControllerBase
{
    private readonly IMediator _mediator;

    public CoversController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // TODO
    //[HttpPost]
    //public ActionResult ComputePremiumAsync(DateOnly startDate, DateOnly endDate, CoverType coverType)
    //{
    //    return Ok(Cover.ComputePremium(startDate, endDate, coverType));
    //}

    [HttpGet]
    public async Task<GetAllCovers.Response> GetAllCovers()
    {
        return await _mediator.Send(new GetAllCovers.Request());
    }

    [HttpGet("{id}")]
    public async Task<GetCover.Response> GetCover(string id)
    {
        var request = new GetCover.Request() { Id = id };
        return await _mediator.Send(request);
    }

    [HttpPost]
    public async Task<CreateCover.Response> CreateCover([FromBody] CreateCover.Request request)
    {
        return await _mediator.Send(request);
    }

    [HttpDelete("{id}")]
    public async Task<DeleteCover.Response> DeleteCover(string id)
    {
        var request = new DeleteCover.Request() { Id = id };
        return await _mediator.Send(request);
    }
}