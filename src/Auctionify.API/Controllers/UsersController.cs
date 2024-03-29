﻿using Auctionify.Application.Common.Models.Requests;
using Auctionify.Application.Features.Rates.Queries.GetPublicUserRates;
using Auctionify.Application.Features.Users.Commands.AddBidForLot;
using Auctionify.Application.Features.Users.Commands.AddLotToWatchlist;
using Auctionify.Application.Features.Users.Commands.Delete;
using Auctionify.Application.Features.Users.Commands.RemoveBid;
using Auctionify.Application.Features.Users.Commands.RemoveLotFromWatchlist;
using Auctionify.Application.Features.Users.Commands.Update;
using Auctionify.Application.Features.Users.Queries.GetAllBidsOfUserForLot;
using Auctionify.Application.Features.Users.Queries.GetBuyer;
using Auctionify.Application.Features.Users.Queries.GetBuyerAuctions;
using Auctionify.Application.Features.Users.Queries.GetById;
using Auctionify.Application.Features.Users.Queries.GetByUserWatchlist;
using Auctionify.Application.Features.Users.Queries.GetSeller;
using Auctionify.Application.Features.Users.Queries.GetTransactions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auctionify.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UsersController : ControllerBase
	{
		private readonly IMediator _mediator;

		public UsersController(IMediator mediator)
		{
			_mediator = mediator;
		}

		[HttpGet("{id}")]
		[Authorize(Roles = "Buyer, Seller")]
		public async Task<IActionResult> GetById(string id)
		{
			var result = await _mediator.Send(new GetByIdUserQuery { Id = id });

			return Ok(result);
		}

		[HttpGet("{userId}/rates")]
		[Authorize(Roles = "Buyer, Seller")]
		public async Task<IActionResult> GetPublicUserRates(string userId)
		{
			var query = new GetPublicUserRatesQuery { UserId = userId };
			var result = await _mediator.Send(query);

			return Ok(result);
		}

		[HttpPost("watchlists/lots")]
		[Authorize(Roles = "Buyer")]
		public async Task<IActionResult> AddToWatchlist(
			[FromForm] AddToWatchlistCommand addToWatchListCommand
		)
		{
			var result = await _mediator.Send(addToWatchListCommand);

			return Ok($"Successfully added the lot to user's watchlist (id: {result.Id})");
		}

		[HttpGet("watchlists/lots")]
		[Authorize(Roles = "Buyer")]
		public async Task<IActionResult> GetUserWatchlist([FromQuery] PageRequest pageRequest)
		{
			var query = new GetWatchlistLotsQuery { PageRequest = pageRequest };
			var lots = await _mediator.Send(query);

			return Ok(lots);
		}

		[HttpDelete("watchlists/lots/{lotId}")]
		[Authorize(Roles = "Buyer")]
		public async Task<IActionResult> RemoveFromWatchlist([FromRoute] int lotId)
		{
			var result = await _mediator.Send(new RemoveLotFromWatchlistCommand { LotId = lotId });

			return Ok(
				$"Successfully removed the lot with id: {lotId} from user's watchlist (id: {result.Id})"
			);
		}

		[HttpPost("bids")]
		[Authorize(Roles = "Buyer")]
		public async Task<IActionResult> AddBidForLot(
			[FromForm] AddBidForLotCommand addBidForLotCommand
		)
		{
			var result = await _mediator.Send(addBidForLotCommand);

			return Ok($"Successfully added the bid for lot (id: {result.Id})");
		}

		[HttpDelete("bids/{bidId}")]
		[Authorize(Roles = "Buyer")]
		public async Task<IActionResult> RemoveBid([FromRoute] int bidId)
		{
			await _mediator.Send(new RemoveBidCommand { BidId = bidId });

			return Ok($"Successfully withdrew the bid with id: {bidId}");
		}

		[HttpGet("lots/{lotId}/bids")]
		[Authorize(Roles = "Buyer")]
		public async Task<IActionResult> GetAllBidsOfUserForLot(
			[FromRoute] int lotId,
			[FromQuery] PageRequest pageRequest
		)
		{
			var query = new GetAllBidsOfUserForLotQuery
			{
				LotId = lotId,
				PageRequest = pageRequest
			};

			var bids = await _mediator.Send(query);

			return Ok(bids);
		}

		[HttpGet("buyers")]
		[Authorize(Roles = "Buyer")]
		public async Task<IActionResult> GetBuyer([FromQuery] PageRequest pageRequest)
		{
			var query = new GetBuyerQuery { PageRequest = pageRequest };
			var result = await _mediator.Send(query);
			return Ok(result);
		}

		[HttpGet("sellers")]
		[Authorize(Roles = "Seller")]
		public async Task<IActionResult> GetSeller([FromQuery] PageRequest pageRequest)
		{
			var query = new GetSellerQuery { PageRequest = pageRequest };
			var result = await _mediator.Send(query);
			return Ok(result);
		}

		[HttpPut]
		[Authorize(Roles = "Buyer, Seller")]
		public async Task<IActionResult> UpdateUserProfile(
			[FromForm] UpdateUserCommand updateUserCommand
		)
		{
			var result = await _mediator.Send(updateUserCommand);
			return Ok(result);
		}

		[HttpGet("buyers/auctions")]
		[Authorize(Roles = "Buyer")]
		public async Task<IActionResult> GetBuyerAuctions([FromQuery] PageRequest pageRequest)
		{
			var query = new GetBuyerAuctionsQuery { PageRequest = pageRequest };
			var lots = await _mediator.Send(query);

			return Ok(lots);
		}

		[HttpGet("transactions")]
		[Authorize(Roles = "Buyer, Seller")]
		public async Task<IActionResult> GetTransactions([FromQuery] PageRequest pageRequest)
		{
			var query = new GetTransactionsUserQuery { PageRequest = pageRequest };
			var transactions = await _mediator.Send(query);

			return Ok(transactions);
		}

		[HttpDelete]
		[Authorize(Roles = "Buyer, Seller")]
		public async Task<IActionResult> DeleteUser()
		{
			var result = await _mediator.Send(new DeleteUserCommand());

			return Ok(result);
		}
	}
}
