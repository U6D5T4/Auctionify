using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Core.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Auctionify.Application.Features.Chats.Commands.CreateChatMessage
{
	public class CreateChatMessageCommandValidator : AbstractValidator<CreateChatMessageCommand>
	{
		private readonly IConversationRepository _conversationRepository;
		private readonly UserManager<User> _userManager;
		private readonly ICurrentUserService _currentUserService;

		public CreateChatMessageCommandValidator(
			IConversationRepository conversationRepository,
			UserManager<User> userManager,
			ICurrentUserService currentUserService
		)
		{
			_conversationRepository = conversationRepository;
			_userManager = userManager;
			_currentUserService = currentUserService;

			ClassLevelCascadeMode = CascadeMode.Stop;

			RuleFor(x => x.Body)
				.Cascade(CascadeMode.Stop)
				.NotEmpty()
				.WithMessage("Body cannot be empty")
				.OverridePropertyName("Body")
				.WithName("Body");

			RuleFor(x => x.ConversationId)
				.Cascade(CascadeMode.Stop)
				.MustAsync(
					async (conversationId, cancellationToken) =>
					{
						var conversation = await _conversationRepository.GetAsync(
							predicate: x => x.Id == conversationId,
							cancellationToken: cancellationToken
						);

						return conversation != null;
					}
				)
				.WithMessage("Conversation with this Id does not exist")
				.OverridePropertyName("ConversationId")
				.WithName("Conversation Id");

			RuleFor(x => x.ConversationId)
				.Cascade(CascadeMode.Stop)
				.MustAsync(
					async (conversationId, cancellationToken) =>
					{
						var users = await _userManager.Users.ToListAsync(
							cancellationToken: cancellationToken
						);
						var user = users.Find(
							u => u.Email == _currentUserService.UserEmail! && !u.IsDeleted
						);

						var conversation = await _conversationRepository.GetAsync(
							predicate: x => x.Id == conversationId,
							cancellationToken: cancellationToken
						);

						return conversation!.BuyerId == user!.Id
							|| conversation.SellerId == user.Id;
					}
				)
				.WithMessage("You are not a part of this conversation")
				.OverridePropertyName("ConversationId")
				.WithName("Conversation Id");
		}
	}
}
