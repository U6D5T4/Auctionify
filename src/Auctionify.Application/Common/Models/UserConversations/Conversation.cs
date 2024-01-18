namespace Auctionify.Application.Common.Models.UserConversations
{
	public class Conversation
	{
		public int Id { get; set; }
		public int LotId { get; set; }
		public ChatUser ChatUser { get; set; }
		public LastMessage LastMessage { get; set; }
		public int UnreadMessagesCount { get; set; }
	}
}
