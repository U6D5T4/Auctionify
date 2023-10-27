using Auctionify.Application.Common.DTOs;
using Auctionify.Application.Features.LotStatuses.Queries.GetLotStatusesForBuyerFiltration;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        [Authorize]
        public async Task<ActionResult<IList<GetLotStatusesForBuyerFiltrationResponse>>> GetAll()
        {
            var resul = await _mediator.Send(new GetLotStatusesForBuyerFiltrationQuery());
            return Ok(resul);
        }
    }
}
