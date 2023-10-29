using Auctionify.Application.Common.Models.Requests;
using Auctionify.Application.Features.Watchlists.Commands.AddLot;
using Auctionify.Application.Features.Watchlists.Queries.GetByUserId;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auctionify.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class WatchlistsController : ControllerBase
	{
		private readonly IMediator _mediator;

		public WatchlistsController(IMediator mediator)
		{
			_mediator = mediator;
		}

		[HttpPost]
		//[Authorize(Roles = "Buyer")]
		public async Task<IActionResult> AddToWatchlist([FromForm] AddToWatchlistCommand addToWatchListCommand)
		{
			var result = await _mediator.Send(addToWatchListCommand);

			return Ok($"Successfully added the lot to user's watchlist (id: {result.Id})");
		}

		[HttpGet("users/{userId}")]
		//[Authorize(Roles = "Buyer")]
		public async Task<IActionResult> GetUserWatchlist([FromRoute] int userId, [FromQuery] PageRequest pageRequest)
		{
			var query = new GetByUserIdWatchlistQuery { UserId = userId, PageRequest = pageRequest };
			var lots = await _mediator.Send(query);

			return Ok(lots);
		}

		[HttpDelete("users/{userId}/lots/{lotId}")]
		[Authorize(Roles = "Buyer")]
		public async Task<IActionResult> RemoveFromWatchlist([FromRoute] int userId, [FromRoute] int lotId)
		{
			
			return Ok();
		}
	}
}
