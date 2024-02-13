using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Core.Entities;
using Auctionify.Core.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Auctionify.Application.Features.Subscriptions.Commands
{
	public class CreateProSubscriptionCommand : IRequest<bool>
	{

	}

	public class CreateProSubscriptionCommandHandler : IRequestHandler<CreateProSubscriptionCommand, bool>
	{
		private readonly ISubscriptionRepository _subscriptionRepository;
		private readonly ICurrentUserService _currentUserService;
		private readonly UserManager<User> _userManager;


		public CreateProSubscriptionCommandHandler(ISubscriptionRepository subscriptionRepository, ICurrentUserService currentUserService, UserManager<User> userManager)
		{
			_subscriptionRepository = subscriptionRepository;
			_currentUserService = currentUserService;
			_userManager = userManager;
		}

		public async Task<bool> Handle(CreateProSubscriptionCommand request, CancellationToken cancellationToken)
		{
			try
			{
				var user = _userManager.Users.FirstOrDefault(u => u.Email == _currentUserService.UserEmail && !u.IsDeleted);

				if (user == null)
					throw new ArgumentException("User is not found");

				var subscription = new Subscription
				{
					CreationDate = DateTime.UtcNow,
					EndDate = DateTime.UtcNow.AddYears(1),
					SubscriptionTypeId = (int)SubscriptionTypeEnum.Pro,
					IsActive = true,
					UserId = user.Id,
				};

				var response = await _subscriptionRepository.AddAsync(subscription);

				if (response == null)
					throw new Exception("Something went wrong when creating subscription");

				return true;
			}
			catch (Exception)
			{

				throw;
			}
		}
	}
}
