using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Core.Entities;
using Auctionify.Core.Enums;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AccountRole = Auctionify.Core.Enums.AccountRole;

namespace Auctionify.Application.Features.Users.Commands.Delete
{
	public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
	{
		private readonly UserManager<User> _userManager;
		private readonly ICurrentUserService _currentUserService;
		private readonly IBidRepository _bidRepository;
		private readonly ILotRepository _lotRepository;

		public DeleteUserCommandValidator(
			UserManager<User> userManager,
			ICurrentUserService currentUserService,
			IBidRepository bidRepository,
			ILotRepository lotRepository
		)
		{
			_userManager = userManager;
			_currentUserService = currentUserService;
			_bidRepository = bidRepository;
			_lotRepository = lotRepository;

			RuleFor(x => x)
				.MustAsync(
					async (command, cancellationToken) =>
					{
						var currentUser = await _userManager.Users.FirstOrDefaultAsync(
							u => u.Email == _currentUserService.UserEmail! && !u.IsDeleted,
							cancellationToken: cancellationToken
						);

						var currentUserRole = (AccountRole)
							Enum.Parse(
								typeof(AccountRole),
								(await _userManager.GetRolesAsync(currentUser!)).FirstOrDefault()!
							);

						if (currentUserRole == AccountRole.Buyer)
						{
							var hasActiveBids = await _bidRepository.AnyAsync(
								predicate: x =>
									x.BuyerId == currentUser!.Id
									&& x.Lot.LotStatus.Name == AuctionStatus.Active.ToString(),
								cancellationToken: cancellationToken
							);

							return !hasActiveBids;
						}
						else if (currentUserRole == AccountRole.Seller)
						{
							var hasInvalidLotStatuses = await _lotRepository.AnyAsync(
								predicate: x =>
									x.SellerId == currentUser!.Id
									&& new[]
									{
										AuctionStatus.Active.ToString(),
										AuctionStatus.Upcoming.ToString(),
										AuctionStatus.PendingApproval.ToString(),
										AuctionStatus.Reopened.ToString()
									}.Contains(x.LotStatus.Name),
								cancellationToken: cancellationToken
							);

							return !hasInvalidLotStatuses;
						}
						return true;
					}
				)
				.WithMessage("You can't delete your account");
		}
	}
}
