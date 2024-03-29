﻿using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Core.Entities;
using Auctionify.Core.Enums;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Auctionify.Application.Features.Chats.Commands.CreateConversation
{
	public class CreateConversationCommandValidator : AbstractValidator<CreateConversationCommand>
	{
		private readonly ILotRepository _lotRepository;
		private readonly ILotStatusRepository _lotStatusRepository;
		private readonly ICurrentUserService _currentUserService;
		private readonly UserManager<User> _userManager;

		public CreateConversationCommandValidator(
			ILotRepository lotRepository,
			ILotStatusRepository lotStatusRepository,
			ICurrentUserService currentUserService,
			UserManager<User> userManager
		)
		{
			_lotRepository = lotRepository;
			_lotStatusRepository = lotStatusRepository;
			_currentUserService = currentUserService;
			_userManager = userManager;

			ClassLevelCascadeMode = CascadeMode.Stop;

			RuleFor(x => x.LotId)
				.Cascade(CascadeMode.Stop)
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
				.WithMessage("Lot with this Id does not exist")
				.OverridePropertyName("LotId")
				.WithName("Lot Id");

			RuleFor(x => x.LotId)
				.Cascade(CascadeMode.Stop)
				.MustAsync(
					async (lotId, cancellationToken) =>
					{
						var lot = await _lotRepository.GetAsync(
							predicate: x => x.Id == lotId,
							cancellationToken: cancellationToken
						);

						var currentUser = await _userManager.Users.FirstOrDefaultAsync(
							u => u.Email == _currentUserService.UserEmail! && !u.IsDeleted,
							cancellationToken: cancellationToken
						);

						var currentUserRoleName = _currentUserService.UserRole!;

						var currentUserRole = (AccountRole)
							Enum.Parse(typeof(AccountRole), currentUserRoleName);

						var buyerId =
							currentUserRole == AccountRole.Buyer ? currentUser!.Id : lot!.BuyerId;

						return buyerId == lot!.BuyerId;
					}
				)
				.WithMessage("You can create a conversation only for auctions you won")
				.OverridePropertyName("LotId")
				.WithName("Lot Id");

			RuleFor(x => x.LotId)
				.Cascade(CascadeMode.Stop)
				.MustAsync(
					async (lotId, cancellationToken) =>
					{
						var lot = await _lotRepository.GetAsync(
							predicate: x => x.Id == lotId,
							cancellationToken: cancellationToken
						);

						var lotStatus = await _lotStatusRepository.GetAsync(
							predicate: x => x.Name == AuctionStatus.Sold.ToString(),
							cancellationToken: cancellationToken
						);

						if (lot is not null && lotStatus is not null && lot.BuyerId is not null)
						{
							return lot.LotStatusId == lotStatus.Id;
						}

						return false;
					}
				)
				.WithMessage("You can create a conversation only for sold lots")
				.OverridePropertyName("LotId")
				.WithName("Lot Id");
		}
	}
}
