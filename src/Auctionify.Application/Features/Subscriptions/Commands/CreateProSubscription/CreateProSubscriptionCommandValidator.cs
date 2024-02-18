using Auctionify.Application.Common.Interfaces;
using Auctionify.Core.Entities;
using Auctionify.Core.Enums;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace Auctionify.Application.Features.Subscriptions.Commands.CreateProSubscription
{
	public class CreateProSubscriptionCommandValidator
		: AbstractValidator<CreateProSubscriptionCommand>
	{
		private readonly UserManager<User> _userManager;
		private readonly ICurrentUserService _currentUserService;

		public CreateProSubscriptionCommandValidator(
			UserManager<User> userManager,
			ICurrentUserService currentUserService
		)
		{
			_userManager = userManager;
			_currentUserService = currentUserService;

			RuleFor(x => x)
				.MustAsync(
					async (item, cancellationToken) =>
					{
						var user = _userManager.Users.FirstOrDefault(
							u => u.Email == _currentUserService.UserEmail && !u.IsDeleted
						);

						if (user == null)
							return false;

						return await _userManager.IsInRoleAsync(user, UserRole.Seller.ToString());
					}
				)
				.WithMessage("User is not a seller!");
		}
	}
}
