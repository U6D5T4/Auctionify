using Auctionify.Application.Features.LotStatuses.Queries.GetLotStatusesForBuyerFiltration;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Auctionify.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LotStatusesController : Controller
    {
        private readonly IMediator _mediator;

        public LotStatusesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<IList<GetLotStatusesForBuyerFiltrationResponse>>> GetAll()
        {
            var result = await _mediator.Send(new GetLotStatusesForBuyerFiltrationQuery());
            return Ok(result);
        }
    }
}
