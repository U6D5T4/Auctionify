using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Core.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace Auctionify.Application.Features.Users.Commands.AddBidForLot
{
	public class AddBidForLotValidator : AbstractValidator<AddBidForLotCommand>
	{
		private readonly IBidRepository _bidRepository;
		private readonly ILotRepository _lotRepository;
		private readonly ICurrentUserService _currentUserService;
		private readonly UserManager<User> _userManager;

		public AddBidForLotValidator(
			IBidRepository bidRepository,
			ILotRepository lotRepository,
			UserManager<User> userManager,
			ICurrentUserService currentUserService
		)
		{
			_bidRepository = bidRepository;
			_lotRepository = lotRepository;
			_userManager = userManager;
			_currentUserService = currentUserService;

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

			RuleFor(x => x.Bid).GreaterThan(0).WithMessage("Bid must be greater than 0");

			RuleFor(x => x)
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

						if (allUserBidsForLot != null && allUserBidsForLot.Items.Count > 0)
						{
							var recentBid = allUserBidsForLot.Items[0];
							return request.Bid > recentBid.NewPrice;
						}

						return true;
					}
				)
				.WithMessage("Bid must be greater than the recent bid");
		}
	}
}
