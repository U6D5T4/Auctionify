using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Core.Entities;
using Auctionify.Core.Enums;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace Auctionify.Application.Features.Users.Commands.RemoveBid
{
	public class RemoveBidCommandValidator : AbstractValidator<RemoveBidCommand>
	{
		private readonly IBidRepository _bidRepository;
		private readonly ILotRepository _lotRepository;
		private readonly ILotStatusRepository _lotStatusRepository;
		private readonly ICurrentUserService _currentUserService;
		private readonly UserManager<User> _userManager;

		public RemoveBidCommandValidator(
			IBidRepository bidRepository,
			ILotRepository lotRepository,
			ILotStatusRepository lotStatusRepository,
			UserManager<User> userManager,
			ICurrentUserService currentUserService
		)
		{
			_bidRepository = bidRepository;
			_lotRepository = lotRepository;
			_lotStatusRepository = lotStatusRepository;
			_userManager = userManager;
			_currentUserService = currentUserService;

			ClassLevelCascadeMode = CascadeMode.Stop;

			RuleFor(x => x.BidId)
				.Cascade(CascadeMode.Stop)
				.MustAsync(
					async (bidId, cancellationToken) =>
					{
						var bid = await _bidRepository.GetAsync(
							predicate: x => x.Id == bidId,
							cancellationToken: cancellationToken
						);

						return bid != null;
					}
				)
				.WithMessage("Bid with this Id does not exist")
				.OverridePropertyName("BidId")
				.WithName("Bid Id");

			RuleFor(x => x.BidId)
				.Cascade(CascadeMode.Stop)
				.MustAsync(
					async (bidId, cancellationToken) =>
					{
						var bid = await _bidRepository.GetAsync(
							predicate: x => x.Id == bidId,
							cancellationToken: cancellationToken
						);

						if (bid is not null)
						{
							var lot = await _lotRepository.GetAsync(
								predicate: x => x.Id == bid!.LotId,
								cancellationToken: cancellationToken
							);

							var lotStatus = await _lotStatusRepository.GetAsync(
								predicate: x => x.Name == AuctionStatus.Active.ToString(),
								cancellationToken: cancellationToken
							);

							if (lot is not null && lotStatus is not null)
							{
								return lot.LotStatusId == lotStatus.Id;
							}

							return false;
						}

						return false;
					}
				)
				.WithMessage("You can withdraw bid only if the lot is active");

			RuleFor(x => x.BidId)
				.Cascade(CascadeMode.Stop)
				.MustAsync(
					async (bidId, cancellationToken) =>
					{
						var user = await _userManager.FindByEmailAsync(
							_currentUserService.UserEmail!
						);

						var bid = await _bidRepository.GetAsync(
							predicate: x => x.Id == bidId,
							cancellationToken: cancellationToken
						);

						if (bid is not null)
						{
							var currentAllUserBidsForLot = await _bidRepository.GetListAsync(
								predicate: x => x.LotId == bid.LotId && x.BuyerId == user!.Id,
								orderBy: x => x.OrderByDescending(x => x.TimeStamp),
								cancellationToken: cancellationToken
							);

							if (currentAllUserBidsForLot!.Items.Any())
							{
								var currentRecentBid = currentAllUserBidsForLot.Items[0];

								return currentRecentBid.Id == bidId;
							}
							return false;
						}
						return false;
					}
				)
				.WithMessage("You can withdraw only your recent bid");

			RuleFor(x => x.BidId)
				.Cascade(CascadeMode.Stop)
				.MustAsync(
					async (bidId, cancellationToken) =>
					{
						var user = await _userManager.FindByEmailAsync(
							_currentUserService.UserEmail!
						);

						var bid = await _bidRepository.GetAsync(
							predicate: x => x.Id == bidId,
							cancellationToken: cancellationToken
						);

						if (bid is not null)
						{
							var currentAllUserBidsForLot = await _bidRepository.GetListAsync(
								predicate: x => x.LotId == bid.LotId && x.BuyerId == user!.Id,
								orderBy: x => x.OrderByDescending(x => x.TimeStamp),
								cancellationToken: cancellationToken
							);

							if (currentAllUserBidsForLot!.Items.Any())
							{
								var count = 0;

								foreach (var currentBid in currentAllUserBidsForLot.Items)
								{
									if (currentBid.BidRemoved)
									{
										count++;
									}
								}

								return count == 0;
							}

							return false;
						}

						return false;
					}
				)
				.WithMessage("You cannot withdraw your bid for lot more than once");
		}
	}
}
