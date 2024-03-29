﻿using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Hubs;
using Auctionify.Core.Entities;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Auctionify.Application.Features.Chats.Commands.CreateChatMessage
{
	public class CreateChatMessageCommand : IRequest<CreatedChatMessageResponse>
	{
		public int ConversationId { get; set; }
		public string Body { get; set; }
	}

	public class CreateChatMessageCommandHandler
		: IRequestHandler<CreateChatMessageCommand, CreatedChatMessageResponse>
	{
		private readonly IMapper _mapper;
		private readonly IChatMessageRepository _chatMessageRepository;
		private readonly ICurrentUserService _currentUserService;
		private readonly UserManager<User> _userManager;
		private readonly IHubContext<AuctionHub> _hubContext;
		private readonly IConversationRepository _conversationRepository;

		public CreateChatMessageCommandHandler(
			IMapper mapper,
			IChatMessageRepository chatMessageRepository,
			ICurrentUserService currentUserService,
			UserManager<User> userManager,
			IHubContext<AuctionHub> hubContext,
			IConversationRepository conversationRepository
		)
		{
			_mapper = mapper;
			_chatMessageRepository = chatMessageRepository;
			_currentUserService = currentUserService;
			_userManager = userManager;
			_hubContext = hubContext;
			_conversationRepository = conversationRepository;
		}

		public async Task<CreatedChatMessageResponse> Handle(
			CreateChatMessageCommand request,
			CancellationToken cancellationToken
		)
		{
			var user = await _userManager.Users.FirstOrDefaultAsync(
				u => u.Email == _currentUserService.UserEmail! && !u.IsDeleted,
				cancellationToken: cancellationToken
			);

			var chatMessage = new ChatMessage
			{
				SenderId = user!.Id,
				ConversationId = request.ConversationId,
				Body = request.Body,
				IsRead = false,
				TimeStamp = DateTime.UtcNow
			};

			var result = await _chatMessageRepository.AddAsync(chatMessage);

			var conversation = await _conversationRepository.GetAsync(
				predicate: x => x.Id == request.ConversationId,
				cancellationToken: cancellationToken
			);

			conversation!.ModificationDate = DateTime.Now;

			await _conversationRepository.UpdateAsync(conversation);

			await _hubContext
				.Clients.Group(request.ConversationId.ToString())
				.SendAsync(
					SignalRActions.ReceiveChatMessageNotification,
					chatMessage.SenderId,
					cancellationToken: cancellationToken
				);

			return _mapper.Map<CreatedChatMessageResponse>(result);
		}
	}
}
