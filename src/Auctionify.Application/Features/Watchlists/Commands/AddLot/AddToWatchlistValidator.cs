using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Core.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace Auctionify.Application.Features.Watchlists.Commands.AddLot
{
	public class AddToWatchlistValidator : AbstractValidator<AddToWatchlistCommand>
	{
		private readonly IWatchlistRepository _watchlistRepository;
		private readonly ILotRepository _lotRepository;
		private readonly UserManager<User> _userManager;

		public AddToWatchlistValidator(
			IWatchlistRepository watchlistRepository,
			ILotRepository lotRepository,
			UserManager<User> userManager
		)
		{
			_watchlistRepository = watchlistRepository;
			_lotRepository = lotRepository;
			_userManager = userManager;

			RuleFor(x => x.LotId)
				.MustAsync(
					async (lotId, cancellationToken) =>
					{
						var lot = await _lotRepository.GetAsync(
							predicate: x => x.Id == lotId,
							cancellationToken: cancellationToken
						);

						return lot != null;
					}
				)
				.WithMessage("Lot with this Id does not exist");

			RuleFor(x => x.UserId)
				.MustAsync(
					async (userId, cancellationToken) =>
					{
						var user = await _userManager.FindByIdAsync(userId.ToString());

						return user != null;
					}
				)
				.WithMessage("User with this Id does not exist");

			RuleFor(x => x)
				.MustAsync(
					async (addToWatchlistCommand, cancellationToken) =>
					{
						var watchlist = await _watchlistRepository.GetAsync(
							predicate: x =>
								x.LotId == addToWatchlistCommand.LotId
								&& x.UserId == addToWatchlistCommand.UserId,
							cancellationToken: cancellationToken
						);

						return watchlist == null;
					}
				)
				.WithMessage("Lot already exists in user's watchlist");
		}
	}
}
