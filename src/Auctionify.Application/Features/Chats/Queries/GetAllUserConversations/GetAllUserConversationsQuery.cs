﻿using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Common.Options;
using Auctionify.Core.Entities;
using Auctionify.Core.Enums;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Auctionify.Application.Features.Chats.Queries.GetAllUserConversations
{
	public class GetAllUserConversationsQuery : IRequest<GetAllUserConversationsResponse> { }

	public class GetAllUserConversationsQueryHandler
		: IRequestHandler<GetAllUserConversationsQuery, GetAllUserConversationsResponse>
	{
		private readonly IConversationRepository _conversationRepository;
		private readonly IMapper _mapper;
		private readonly ICurrentUserService _currentUserService;
		private readonly UserManager<User> _userManager;
		private readonly IBlobService _blobService;
		private readonly AzureBlobStorageOptions _azureBlobStorageOptions;

		public GetAllUserConversationsQueryHandler(
			IConversationRepository conversationRepository,
			IMapper mapper,
			ICurrentUserService currentUserService,
			UserManager<User> userManager,
			IBlobService blobService,
			IOptions<AzureBlobStorageOptions> azureBlobStorageOptions
		)
		{
			_conversationRepository = conversationRepository;
			_mapper = mapper;
			_currentUserService = currentUserService;
			_userManager = userManager;
			_blobService = blobService;
			_azureBlobStorageOptions = azureBlobStorageOptions.Value;
		}

		public async Task<GetAllUserConversationsResponse> Handle(
			GetAllUserConversationsQuery request,
			CancellationToken cancellationToken
		)
		{
			#region Getting the current user's information

			var currentUser = await _userManager.FindByEmailAsync(_currentUserService.UserEmail!);
			var currentUserRole = (UserRole)
				Enum.Parse(
					typeof(UserRole),
					(await _userManager.GetRolesAsync(currentUser!)).FirstOrDefault()!
				);

			#endregion

			#region Creating the user object with the current user's information and profile picture

			var user = new Common.Models.UserConversations.User
			{
				Id = currentUser!.Id,
				FullName = $"{currentUser!.FirstName} {currentUser!.LastName}",
				Role = currentUserRole.ToString()
			};

			var currentUserProfilePictureName = currentUser!.ProfilePicture;

			if (currentUserProfilePictureName != null)
			{
				var currentUserProfilePictureUrl = _blobService.GetBlobUrl(
					_azureBlobStorageOptions.UserProfilePhotosFolderName,
					currentUserProfilePictureName
				);

				user.ProfilePictureUrl = currentUserProfilePictureUrl;
			}

			#endregion

			#region Getting the current user's conversations

			var currentUserConversations = await _conversationRepository.GetUnpaginatedListAsync(
				predicate: c => c.BuyerId == user!.Id || c.SellerId == user!.Id,
				include: c =>
					c.Include(c => c.Buyer)
						.Include(c => c.Seller)
						.Include(c => c.Lot)
						.Include(c => c.ChatMessages),
				cancellationToken: cancellationToken
			);

			#endregion

			#region Creating the user's conversations list with attached last message and unread messages count

			var userConversationsList = new List<Common.Models.UserConversations.Conversation>();

			foreach (var conversation in currentUserConversations)
			{
				var userConversation = new Common.Models.UserConversations.Conversation
				{
					Id = conversation.Id,
					LotId = conversation.LotId,
					ChatUser = new Common.Models.UserConversations.ChatUser
					{
						Id =
							conversation.BuyerId == user.Id
								? conversation.SellerId
								: conversation.BuyerId,
						FullName =
							conversation.BuyerId == user.Id
								? $"{conversation.Seller.FirstName} {conversation.Seller.LastName}"
								: $"{conversation.Buyer.FirstName} {conversation.Buyer.LastName}",
						Role =
							conversation.BuyerId == user.Id
								? UserRole.Seller.ToString()
								: UserRole.Buyer.ToString()
					},
					LastMessage = new Common.Models.UserConversations.LastMessage
					{
						Id = conversation.ChatMessages.Last().Id,
						SenderId = conversation.ChatMessages.Last().SenderId,
						ConversationId = conversation.ChatMessages.Last().ConversationId,
						Body = conversation.ChatMessages.Last().Body,
						TimeStamp = conversation.ChatMessages.Last().TimeStamp,
						IsRead = conversation.ChatMessages.Last().IsRead
					},
					UnreadMessagesCount = conversation.ChatMessages.Count(
						cm => cm.SenderId != user.Id && !cm.IsRead
					)
				};

				userConversationsList.Add(userConversation);
			}

			#endregion

			var response = new Common.Models.UserConversations.UserConversations
			{
				User = user,
				Conversations = userConversationsList
			};

			return _mapper.Map<GetAllUserConversationsResponse>(response);
		}
	}
}