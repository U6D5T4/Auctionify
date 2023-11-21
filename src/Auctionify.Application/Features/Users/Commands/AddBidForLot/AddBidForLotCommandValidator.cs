using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Core.Entities;
using Auctionify.Core.Enums;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace Auctionify.Application.Features.Users.Commands.AddBidForLot
{
	public class AddBidForLotCommandValidator : AbstractValidator<AddBidForLotCommand>
	{
		private readonly IBidRepository _bidRepository;
		private readonly ILotRepository _lotRepository;
		private readonly ILotStatusRepository _lotStatusRepository;
		private readonly ICurrentUserService _currentUserService;
		private readonly UserManager<User> _userManager;

		public AddBidForLotCommandValidator(
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

			RuleFor(x => x.LotId)
				.Cascade(CascadeMode.Stop)
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
				.WithMessage("Lot with this Id does not exist")
				.OverridePropertyName("LotId")
				.WithName("Lot Id");

			RuleFor(x => x.LotId)
				.Cascade(CascadeMode.Stop)
				.MustAsync(
					async (lotId, cancellationToken) =>
					{
						var lot = await _lotRepository.GetAsync(
							predicate: x => x.Id == lotId,
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
				)
				.WithMessage("You can bid only if the lot is active")
				.OverridePropertyName("LotId")
				.WithName("Lot Id");

			RuleFor(x => x.Bid)
				.Cascade(CascadeMode.Stop)
				.GreaterThan(0)
				.WithMessage("Bid must be greater than 0 and be a positive number")
				.OverridePropertyName("Bid")
				.WithName("Bid");

			RuleFor(x => x)
				.Cascade(CascadeMode.Stop)
				.MustAsync(
					async (request, cancellationToken) =>
					{
						var lot = await _lotRepository.GetAsync(
							predicate: x => x.Id == request.LotId,
							cancellationToken: cancellationToken
						);

						if (lot is not null && request.Bid <= lot.StartingPrice)
						{
							return false;
						}

						return true;
					}
				)
				.WithMessage("Bid must be greater than the lot's starting price")
				.OverridePropertyName("Bid")
				.WithName("Bid");

			RuleFor(x => x)
				.Cascade(CascadeMode.Stop)
				.MustAsync(
					async (request, cancellationToken) =>
					{
						var user = await _userManager.FindByEmailAsync(
							_currentUserService.UserEmail!
						);
						var allUserBidsForLot = await _bidRepository.GetListAsync(
							predicate: x => x.LotId == request.LotId && x.BuyerId == user!.Id,
							orderBy: x => x.OrderByDescending(x => x.TimeStamp),
							cancellationToken: cancellationToken
						);

						if (allUserBidsForLot is not null && allUserBidsForLot.Items.Count > 0)
						{
							var previousBid = allUserBidsForLot.Items[0];
							return request.Bid > previousBid.NewPrice;
						}

						return true;
					}
				)
				.WithMessage("Your new bid must be greater than your previous bid")
				.OverridePropertyName("Bid")
				.WithName("Bid");

			RuleFor(x => x)
				.Cascade(CascadeMode.Stop)
				.MustAsync(
					async (request, cancellationToken) =>
					{
						var lot = await _lotRepository.GetAsync(
							predicate: x => x.Id == request.LotId,
							cancellationToken: cancellationToken
						);

						if (lot is not null)
						{
							var currentLotBids = await _bidRepository.GetListAsync(
								predicate: x => x.LotId == request.LotId,
								orderBy: x => x.OrderByDescending(x => x.TimeStamp),
								cancellationToken: cancellationToken
							);

							if (currentLotBids is not null && currentLotBids.Items.Count > 0)
							{
								var currentBidPrice = currentLotBids.Items[0].NewPrice;
								return request.Bid > currentBidPrice;
							}
						}

						return true;
					}
				)
				.WithMessage("Bid must be greater than the current bid of the lot")
				.OverridePropertyName("Bid")
				.WithName("Bid");
		}
	}
}
