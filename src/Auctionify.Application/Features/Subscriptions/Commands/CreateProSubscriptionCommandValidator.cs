using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Core.Entities;
using Auctionify.Core.Enums;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace Auctionify.Application.Features.Subscriptions.Commands
{
    public class CreateProSubscriptionCommandValidator : AbstractValidator<CreateProSubscriptionCommand>
    {
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly ICurrentUserService _currentUserService;

        public CreateProSubscriptionCommandValidator(ISubscriptionRepository subscriptionRepository,
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            ICurrentUserService currentUserService)
        {
            _subscriptionRepository = subscriptionRepository;
            _userManager = userManager;
            _roleManager = roleManager;
            _currentUserService = currentUserService;

            RuleFor(x => x)
                .MustAsync(
                    async (item, cancellationToken) =>
                    {
                        var userEmail = _currentUserService.UserEmail;
                        var user = _userManager.Users.FirstOrDefault(u => u.Email == _currentUserService.UserEmail && !u.IsDeleted);

                        if (user == null)
                            return false;

                        return await _userManager.IsInRoleAsync(user, UserRole.Seller.ToString());
                    }
                )
                .WithMessage("User is not a seller!");
        }
    }
}
