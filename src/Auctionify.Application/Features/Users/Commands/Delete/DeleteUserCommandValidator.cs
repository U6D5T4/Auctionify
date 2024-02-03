using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Core.Entities;
using Auctionify.Core.Enums;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserRole = Auctionify.Core.Enums.UserRole;

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
						var currentUser = await _userManager.Users.FirstOrDefaultAsync(
							u => u.Email == _currentUserService.UserEmail! && !u.IsDeleted,
							cancellationToken: cancellationToken
						);

						var currentUserRole = (UserRole)
							Enum.Parse(
								typeof(UserRole),
								(await _userManager.GetRolesAsync(currentUser!)).FirstOrDefault()!
							);

						if (currentUserRole == UserRole.Buyer)
						{
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
						}
						else if (currentUserRole == UserRole.Seller)
						{
							var lots = await _lotRepository.GetUnpaginatedListAsync(
								predicate: x => x.SellerId == currentUser!.Id,
								cancellationToken: cancellationToken
							);

							foreach (var lot in lots)
							{
								var lotStatus = await _lotStatusRepository.GetAsync(
									predicate: x => x.Id == lot!.LotStatusId,
									cancellationToken: cancellationToken
								);

								var invalidLotStatuses = new List<string>
								{
									AuctionStatus.Active.ToString(),
									AuctionStatus.Upcoming.ToString(),
									AuctionStatus.PendingApproval.ToString(),
									AuctionStatus.Reopened.ToString()
								};

								if (invalidLotStatuses.Contains(lotStatus!.Name))
								{
									return false;
								}
							}
						}
						return true;
					}
				)
				.WithMessage("You can't delete your account");
		}
	}
}
