using Auctionify.Application.Common.Models.Requests;
using Auctionify.Application.Features.Rates.Queries.GetReceiverRates;
using Auctionify.Application.Features.Rates.Queries.GetSenderRates;
using Auctionify.Application.Features.Rates.Commands.AddRateToBuyer;
using Auctionify.Application.Features.Rates.Commands.AddRateToSeller;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auctionify.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class RatesController : ControllerBase
	{
		private readonly IMediator _mediator;

		public RatesController(IMediator mediator)
		{
			_mediator = mediator;
		}

		[HttpPost("buyers")]
		[Authorize(Roles = "Seller")]
		public async Task<IActionResult> RateBuyer(
			[FromForm] AddRateToBuyerCommand addRateToBuyerCommand
		)
		{
			var result = await _mediator.Send(addRateToBuyerCommand);

			return Ok($"Successfully added the rate for buyer (id: {result.Id})");
		}

		[HttpPost("sellers")]
		[Authorize(Roles = "Buyer")]
		public async Task<IActionResult> RateSeller(
			[FromForm] AddRateToSellerCommand addRateToSellerCommand
		)
		{
			var result = await _mediator.Send(addRateToSellerCommand);

			return Ok($"Successfully added the rate for seller (id: {result.Id})");
		}

		[HttpGet("rates")]
		[Authorize(Roles = "Buyer, Seller")]
		public async Task<IActionResult> GetRates([FromQuery] PageRequest pageRequest)
		{
			var query = new GetAllSenderRatesQuery { PageRequest = pageRequest };
			var result = await _mediator.Send(query);
			return Ok(result);
		}

		[HttpGet("feedbacks")]
		[Authorize(Roles = "Buyer, Seller")]
		public async Task<IActionResult> GetFeedbacks([FromQuery] PageRequest pageRequest)
		{
			var query = new GetAllReceiverRatesQuery { PageRequest = pageRequest };
			var result = await _mediator.Send(query);
			return Ok(result);
		}
	}
}
