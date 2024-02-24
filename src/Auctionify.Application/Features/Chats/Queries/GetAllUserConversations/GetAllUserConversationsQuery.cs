using Auctionify.Application.Common.Interfaces;
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

			var currentUser = await _userManager.Users.FirstOrDefaultAsync(
				u => u.Email == _currentUserService.UserEmail! && !u.IsDeleted,
				cancellationToken: cancellationToken
			);

			var currentUserRoleName = _currentUserService.UserRole!;

			var currentUserRole = (AccountRole)Enum.Parse(typeof(AccountRole), currentUserRoleName);

			#endregion

			#region Creating the user object with the current user's information and profile picture

			var user = new Common.Models.UserConversations.User
			{
				Id = currentUser!.Id,
				FullName = $"{currentUser!.FirstName} {currentUser!.LastName}",
				Role = currentUserRole.ToString(),
				Email = currentUser!.Email!
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
				orderBy: c => c.OrderByDescending(c => c.ModificationDate),
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
						Email =
							conversation.BuyerId == user.Id
								? conversation.Seller!.Email!
								: conversation.Buyer!.Email!,
						Role =
							conversation.BuyerId == user.Id
								? AccountRole.Seller.ToString()
								: AccountRole.Buyer.ToString(),
						ProfilePictureUrl =
							conversation.BuyerId == user.Id
								? _blobService.GetBlobUrl(
									_azureBlobStorageOptions.UserProfilePhotosFolderName,
									conversation.Seller!.ProfilePicture
								)
								: _blobService.GetBlobUrl(
									_azureBlobStorageOptions.UserProfilePhotosFolderName,
									conversation.Buyer!.ProfilePicture
								)
					},
					LastMessage = conversation.ChatMessages.Any()
						? new Common.Models.UserConversations.LastMessage
						{
							Id = conversation.ChatMessages.Last().Id,
							SenderId = conversation.ChatMessages.Last().SenderId,
							ConversationId = conversation.ChatMessages.Last().ConversationId,
							Body = conversation.ChatMessages.Last().Body,
							TimeStamp = conversation.ChatMessages.Last().TimeStamp,
							IsRead = conversation.ChatMessages.Last().IsRead
						}
						: new Common.Models.UserConversations.LastMessage { },
					UnreadMessagesCount = conversation.ChatMessages.Count(
						cm => cm.SenderId != user.Id && !cm.IsRead
					),
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
