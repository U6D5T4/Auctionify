using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Auctionify.Core.Entities;

namespace Auctionify.Application.Features.Subscriptions.Commands.DeleteProSubscription
{
    public class DeleteProSubscriptionCommand : IRequest<bool>
    {

    }

    public class DeleteProSubscriptionCommandHandler : IRequestHandler<DeleteProSubscriptionCommand, bool>
    {
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly UserManager<User> _userManager;

        public DeleteProSubscriptionCommandHandler(ISubscriptionRepository subscriptionRepository,
            ICurrentUserService currentUserService,
            UserManager<User> userManager)
        {
            _subscriptionRepository = subscriptionRepository;
            _currentUserService = currentUserService;
            _userManager = userManager;
        }

        public async Task<bool> Handle(DeleteProSubscriptionCommand request, CancellationToken cancellationToken)
        {
            var user = _userManager.Users.FirstOrDefault(u => u.Email == _currentUserService.UserEmail && !u.IsDeleted);

            if (user == null)
                throw new ArgumentException("User is not found");

            var userSubscription = await _subscriptionRepository.GetAsync(s => s.User.Id == user.Id);

            if (userSubscription == null)
                throw new Exception("User does not have any subscriptions");

            await _subscriptionRepository.DeleteAsync(userSubscription);

            return true;
        }
    }

}
