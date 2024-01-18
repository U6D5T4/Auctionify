using Auctionify.Application.Common.Models.UserConversations;

namespace Auctionify.Application.Features.Chats.Queries.GetAllUserConversations
{
	public class GetAllUserConversationsResponse
	{
		public User User { get; set; }
		public List<Conversation> Conversations { get; set; }
	}
}
