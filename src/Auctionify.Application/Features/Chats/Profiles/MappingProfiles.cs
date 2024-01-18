using Auctionify.Application.Common.Models.UserConversations;
using Auctionify.Application.Features.Chats.Commands.CreateChatMessage;
using Auctionify.Application.Features.Chats.Commands.CreateConversation;
using Auctionify.Application.Features.Chats.Queries.GetAllUserConversations;
using Auctionify.Core.Entities;
using AutoMapper;

namespace Auctionify.Application.Features.Chats.Profiles
{
	public class MappingProfiles : Profile
	{
		public MappingProfiles()
		{
			CreateMap<UserConversations, GetAllUserConversationsResponse>().ReverseMap();
			CreateMap<ChatMessage, CreatedChatMessageResponse>().ReverseMap();
			CreateMap<Core.Entities.Conversation, CreatedConversationResponse>().ReverseMap();
		}
	}
}
