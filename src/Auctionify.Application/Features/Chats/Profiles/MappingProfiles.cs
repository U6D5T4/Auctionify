using Auctionify.Application.Common.Models.UserConversations;
using Auctionify.Application.Features.Chats.Queries.GetAllUserConversations;
using AutoMapper;

namespace Auctionify.Application.Features.Chats.Profiles
{
	public class MappingProfiles : Profile
	{
		public MappingProfiles()
		{
			CreateMap<UserConversations, GetAllUserConversationsResponse>().ReverseMap();
		}
	}
}