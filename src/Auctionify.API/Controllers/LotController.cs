using Auctionify.Application.Features.Lots.Queries.GetAllLots;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Auctionify.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class LotController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LotController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // api/v1/auth/getalllots
        [HttpGet("GetAllLots")]
        public async Task<IActionResult> GetAllLots()
        {
            try
            {
                var query = new GetAllLotsQuery();
                var lots = await _mediator.Send(query);

                return Ok(lots);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
    }
}
