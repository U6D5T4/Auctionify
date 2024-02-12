using Auctionify.Application.Features.Subscriptions.Commands;
using Auctionify.Application.Features.Subscriptions.Commands.DeleteProSubscription;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auctionify.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionsController : Controller
    {
        private readonly IMediator _mediator;

        public SubscriptionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("pro/create")]
        [Authorize(Roles = "Seller")]
        public async Task<ActionResult<bool>> CreateProSubscription()
        {
            var result = await _mediator.Send(new CreateProSubscriptionCommand());

            return Ok(result);
        }

        [HttpDelete("pro/delete")]
        [Authorize(Roles = "Seller")]
        public async Task<ActionResult<bool>> DeleteProSubscription()
        {
            var result = await _mediator.Send(new DeleteProSubscriptionCommand());

            return Ok(result);
        }
    }
}
