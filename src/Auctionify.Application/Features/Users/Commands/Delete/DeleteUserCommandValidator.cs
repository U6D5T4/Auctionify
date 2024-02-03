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

						var currentUserRole = (UserRole)
							Enum.Parse(
								typeof(UserRole),
								(await _userManager.GetRolesAsync(currentUser!)).FirstOrDefault()!
							);

						if (currentUserRole == UserRole.Buyer)
						{
							var bids = await _bidRepository.GetUnpaginatedListAsync(
								predicate: x => x.BuyerId == currentUser!.Id,
								include: x => x.Include(b => b.Lot).ThenInclude(l => l.LotStatus),
								cancellationToken: cancellationToken
							);

							return bids.All(
								bid => bid.Lot.LotStatus.Name != AuctionStatus.Active.ToString()
							);
						}
						else if (currentUserRole == UserRole.Seller)
						{
							var lots = await _lotRepository.GetUnpaginatedListAsync(
								predicate: x => x.SellerId == currentUser!.Id,
								include: x => x.Include(l => l.LotStatus),
								cancellationToken: cancellationToken
							);

							var invalidLotStatuses = new List<string>
							{
								AuctionStatus.Active.ToString(),
								AuctionStatus.Upcoming.ToString(),
								AuctionStatus.PendingApproval.ToString(),
								AuctionStatus.Reopened.ToString()
							};

							return lots.All(
								lot => !invalidLotStatuses.Contains(lot.LotStatus.Name)
							);
						}
						return true;
					}
				)
				.WithMessage("You can't delete your account");
		}
	}
}
