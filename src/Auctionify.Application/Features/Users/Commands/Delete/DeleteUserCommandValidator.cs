using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Core.Entities;
using Auctionify.Core.Enums;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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

			RuleFor(x => x)
				.Cascade(CascadeMode.Stop)
				.MustAsync(
					async (command, cancellationToken) =>
					{
						var users = await _userManager.Users.ToListAsync(
							cancellationToken: cancellationToken
						);
						var currentUser = users.Find(
							u => u.Email == _currentUserService.UserEmail! && !u.IsDeleted
						);

						var currentUserRole = (UserRole)
							Enum.Parse(
								typeof(UserRole),
								(await _userManager.GetRolesAsync(currentUser!)).FirstOrDefault()!
							);

						if (currentUserRole != UserRole.Buyer)
						{
							return true;
						}

						var bids = await _bidRepository.GetUnpaginatedListAsync(
							predicate: x => x.BuyerId == currentUser!.Id,
							cancellationToken: cancellationToken
						);

						foreach (var bid in bids)
						{
							var lot = await _lotRepository.GetAsync(
								predicate: x => x.Id == bid.LotId,
								cancellationToken: cancellationToken
							);

							var lotStatus = await _lotStatusRepository.GetAsync(
								predicate: x => x.Id == lot!.LotStatusId,
								cancellationToken: cancellationToken
							);

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

			RuleFor(x => x)
				.Cascade(CascadeMode.Stop)
				.MustAsync(
					async (command, cancellationToken) =>
					{
						var users = await _userManager.Users.ToListAsync(
							cancellationToken: cancellationToken
						);

						var currentUser = users.Find(
							u => u.Email == _currentUserService.UserEmail! && !u.IsDeleted
						);

						var currentUserRole = (UserRole)
							Enum.Parse(
								typeof(UserRole),
								(await _userManager.GetRolesAsync(currentUser!)).FirstOrDefault()!
							);

						if (currentUserRole != UserRole.Seller)
						{
							return true;
						}

						var lots = await _lotRepository.GetUnpaginatedListAsync(
							predicate: x => x.SellerId == currentUser!.Id,
							cancellationToken: cancellationToken
						);

						foreach (var lot in lots)
						{
							var lotStatus = await _lotStatusRepository.GetAsync(
								predicate: x => x.Id == lot.LotStatusId,
								cancellationToken: cancellationToken
							);

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
