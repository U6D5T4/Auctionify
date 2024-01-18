using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Core.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace Auctionify.Application.Features.Chats.Commands.MarkChatMessageAsRead
{
	public class MarkChatMessageAsReadCommandValidator
		: AbstractValidator<MarkChatMessageAsReadCommand>
	{
		private readonly IChatMessageRepository _chatMessageRepository;
		private readonly IConversationRepository _conversationRepository;
		private readonly UserManager<User> _userManager;
		private readonly ICurrentUserService _currentUserService;

		public MarkChatMessageAsReadCommandValidator(
			IChatMessageRepository chatMessageRepository,
			IConversationRepository conversationRepository,
			UserManager<User> userManager,
			ICurrentUserService currentUserService
		)
		{
			_chatMessageRepository = chatMessageRepository;
			_conversationRepository = conversationRepository;
			_userManager = userManager;
			_currentUserService = currentUserService;

			ClassLevelCascadeMode = CascadeMode.Stop;

			RuleFor(x => x.ChatMessageId)
				.Cascade(CascadeMode.Stop)
				.MustAsync(
					async (chatMessageId, cancellationToken) =>
					{
						var chatMessage = await _chatMessageRepository.GetAsync(
							predicate: x => x.Id == chatMessageId,
							cancellationToken: cancellationToken
						);

						return chatMessage != null;
					}
				)
				.WithMessage("Chat message with this Id does not exist")
				.OverridePropertyName("ChatMessageId")
				.WithName("Chat Message Id");

			RuleFor(x => x.ChatMessageId)
				.Cascade(CascadeMode.Stop)
				.MustAsync(
					async (chatMessageId, cancellationToken) =>
					{
						var chatMessage = await _chatMessageRepository.GetAsync(
							predicate: x => x.Id == chatMessageId,
							cancellationToken: cancellationToken
						);

						var user = await _userManager.FindByEmailAsync(
							_currentUserService.UserEmail!
						);

						return chatMessage!.SenderId != user!.Id;
					}
				)
				.WithMessage("Sender of the message cannot mark it as read")
				.OverridePropertyName("ChatMessageId")
				.WithName("Chat Message Id");

			RuleFor(x => x.ChatMessageId)
				.Cascade(CascadeMode.Stop)
				.MustAsync(
					async (chatMessageId, cancellationToken) =>
					{
						var chatMessage = await _chatMessageRepository.GetAsync(
							predicate: x => x.Id == chatMessageId,
							cancellationToken: cancellationToken
						);

						var user = await _userManager.FindByEmailAsync(
							_currentUserService.UserEmail!
						);

						var conversation = await _conversationRepository.GetAsync(
							predicate: x => x.Id == chatMessage!.ConversationId,
							cancellationToken: cancellationToken
						);

						return conversation!.BuyerId == user!.Id
							|| conversation.SellerId == user.Id;
					}
				)
				.WithMessage("You are not the receiver of this chat message")
				.OverridePropertyName("ChatMessageId")
				.WithName("Chat Message Id");
		}
	}
}
