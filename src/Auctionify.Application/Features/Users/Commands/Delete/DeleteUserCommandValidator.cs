using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Core.Entities;
using Auctionify.Core.Enums;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace Auctionify.Application.Features.Users.Commands.Delete
{
	public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
	{
		private readonly UserManager<User> _userManager;
		private readonly ICurrentUserService _currentUserService;
		private readonly IBidRepository _bidRepository;
		private readonly ILotRepository _lotRepository;
		private readonly ILotStatusRepository _lotStatusRepository;

		public DeleteUserCommandValidator(
			UserManager<User> userManager,
			ICurrentUserService currentUserService,
			IBidRepository bidRepository,
			ILotRepository lotRepository,
			ILotStatusRepository lotStatusRepository
		)
		{
			_userManager = userManager;
			_currentUserService = currentUserService;
			_bidRepository = bidRepository;
			_lotRepository = lotRepository;
			_lotStatusRepository = lotStatusRepository;

			ClassLevelCascadeMode = CascadeMode.Stop;

			// for buyer role
			// Buyer can’t delete his account if he has at least one active bid
			RuleFor(x => x)
				.Cascade(CascadeMode.Stop)
				.MustAsync(
					async (command, cancellationToken) =>
					{
						// get current user
						var currentUser = await _userManager.FindByEmailAsync(
							_currentUserService.UserEmail!
						);

						// get current user role
						var currentUserRole = (UserRole)
							Enum.Parse(
								typeof(UserRole),
								(await _userManager.GetRolesAsync(currentUser!)).FirstOrDefault()!
							);

						// if the current user is not a buyer, then we just skip this validation
						if (currentUserRole != UserRole.Buyer)
						{
							return true;
						}

						// get all bids of the buyer
						var bids = await _bidRepository.GetUnpaginatedListAsync(
							predicate: x => x.BuyerId == currentUser!.Id,
							cancellationToken: cancellationToken
						);

						// for each bid, check if the lot is active
						foreach (var bid in bids)
						{
							// get the lot
							var lot = await _lotRepository.GetAsync(
								predicate: x => x.Id == bid.LotId,
								cancellationToken: cancellationToken
							);

							// get the lot status
							var lotStatus = await _lotStatusRepository.GetAsync(
								predicate: x => x.Id == lot!.LotStatusId,
								cancellationToken: cancellationToken
							);

							// if the lot status is active, then the buyer can't delete his account
							if (lotStatus!.Name == AuctionStatus.Active.ToString())
							{
								return false;
							}
						}

						return true;
					}
				)
				.WithMessage("You can't delete your account if you have at least one active bid")
				.OverridePropertyName("BuyerId")
				.WithName("Buyer Id");

			// Inactive states/statuses for now, if I haven't forgotten anything:
			// successful purchase by buyer, (lot status: sold)
			// no purchase, (lot status: not sold)
			// archived status, (lot status: archived)
			// draft status (lot status: draft)
			// in above cases, the seller can delete his account

			// for seller role
			// Seller can’t delete his account if he has at least one active lot
			RuleFor(x => x)
				.Cascade(CascadeMode.Stop)
				.MustAsync(
					async (command, cancellationToken) =>
					{
						// get current user
						var currentUser = await _userManager.FindByEmailAsync(
							_currentUserService.UserEmail!
						);

						// get current user role
						var currentUserRole = (UserRole)
							Enum.Parse(
								typeof(UserRole),
								(await _userManager.GetRolesAsync(currentUser!)).FirstOrDefault()!
							);

						// if the current user is not a seller, then we just skip this validation
						if (currentUserRole != UserRole.Seller)
						{
							return true;
						}

						// get all lots of the seller
						var lots = await _lotRepository.GetUnpaginatedListAsync(
							predicate: x => x.SellerId == currentUser!.Id,
							cancellationToken: cancellationToken
						);

						// for each lot, check if the lot is active
						foreach (var lot in lots)
						{
							// get the lot status
							var lotStatus = await _lotStatusRepository.GetAsync(
								predicate: x => x.Id == lot.LotStatusId,
								cancellationToken: cancellationToken
							);

							// if the lot status is active, upcoming, pendingapproval, reopened,
							// then the seller can't delete his account
							if (
								lotStatus!.Name == AuctionStatus.Active.ToString()
								|| lotStatus!.Name == AuctionStatus.Upcoming.ToString()
								|| lotStatus!.Name == AuctionStatus.PendingApproval.ToString()
								|| lotStatus!.Name == AuctionStatus.Reopened.ToString()
							)
							{
								return false;
							}
						}

						return true;
					}
				)
				.WithMessage(
					"You can't delete your account if you have at least one lot with either active, upcoming, pending approval or reopened status"
				)
				.OverridePropertyName("SellerId")
				.WithName("Seller Id");
		}
	}
}
