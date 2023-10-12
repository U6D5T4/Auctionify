using Auctionify.Application.Features.Lots.Queries.GetAllLots;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Auctionify.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LotsController : ControllerBase
    {
        private readonly IMediator mediator;

        public LotsController(IMediator mediator)
        {
            
            this.mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetAllLotsQuery();
            var lots = await mediator.Send(query);

            return Ok(lots);
        }
    }
}
