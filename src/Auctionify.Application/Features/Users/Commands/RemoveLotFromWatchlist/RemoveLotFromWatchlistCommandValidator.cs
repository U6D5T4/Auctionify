using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Core.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Auctionify.Application.Features.Users.Commands.RemoveLotFromWatchlist
{
	public class RemoveLotFromWatchlistCommandValidator
		: AbstractValidator<RemoveLotFromWatchlistCommand>
	{
		private readonly IWatchlistRepository _watchlistRepository;
		private readonly ILotRepository _lotRepository;
		private readonly ICurrentUserService _currentUserService;
		private readonly UserManager<User> _userManager;

		public RemoveLotFromWatchlistCommandValidator(
			IWatchlistRepository watchlistRepository,
			ILotRepository lotRepository,
			ICurrentUserService currentUserService,
			UserManager<User> userManager
		)
		{
			_watchlistRepository = watchlistRepository;
			_lotRepository = lotRepository;
			_currentUserService = currentUserService;
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

			RuleFor(x => x.LotId)
				.MustAsync(
					async (lotId, cancellationToken) =>
					{
						var user = await _userManager.Users.FirstOrDefaultAsync(
							u => u.Email == _currentUserService.UserEmail! && !u.IsDeleted,
							cancellationToken: cancellationToken
						);

						var watchlist = await _watchlistRepository.GetAsync(
							predicate: x => x.LotId == lotId && x.UserId == user!.Id,
							cancellationToken: cancellationToken
						);

						return watchlist != null;
					}
				)
				.WithMessage("User does not have this lot in his watchlist");
		}
	}
}
