using Auctionify.Application.Common.DTOs;
using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Core.Entities;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Auctionify.Application.Features.Chats.Queries.GetAllChatMessages
{
	public class GetAllChatMessagesQuery : IRequest<GetAllChatMessagesResponse>
	{
		public int ConversationId { get; set; }
	}

	public class GetAllChatMessagesQueryHandler
		: IRequestHandler<GetAllChatMessagesQuery, GetAllChatMessagesResponse>
	{
		private readonly IConversationRepository _conversationRepository;
		private readonly IChatMessageRepository _chatMessageRepository;
		private readonly IMapper _mapper;
		private readonly ICurrentUserService _currentUserService;
		private readonly UserManager<User> _userManager;

		public GetAllChatMessagesQueryHandler(
			IConversationRepository conversationRepository,
			IChatMessageRepository chatMessageRepository,
			IMapper mapper,
			ICurrentUserService currentUserService,
			UserManager<User> userManager
		)
		{
			_conversationRepository = conversationRepository;
			_chatMessageRepository = chatMessageRepository;
			_mapper = mapper;
			_currentUserService = currentUserService;
			_userManager = userManager;
		}

		public async Task<GetAllChatMessagesResponse> Handle(
			GetAllChatMessagesQuery request,
			CancellationToken cancellationToken
		)
		{
			var currentUser = await _userManager.FindByEmailAsync(_currentUserService.UserEmail!);

			var conversation =
				await _conversationRepository.GetAsync(
					predicate: c =>
						c.Id == request.ConversationId
						&& (c.BuyerId == currentUser!.Id || c.SellerId == currentUser!.Id),
					cancellationToken: cancellationToken
				)
				?? throw new ValidationException(
					new List<ValidationFailure>
					{
						new("ConversationId", "You are not allowed to access this conversation")
					}
				);

			var chatMessages = await _chatMessageRepository.GetUnpaginatedListAsync(
				predicate: m => m.ConversationId == conversation.Id,
				cancellationToken: cancellationToken
			);

			var response = new GetAllChatMessagesResponse
			{
				ChatMessages = _mapper.Map<List<ChatMessageDto>>(chatMessages)
			};

			return response;
		}
	}
}
