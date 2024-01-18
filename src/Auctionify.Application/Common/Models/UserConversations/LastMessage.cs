namespace Auctionify.Application.Common.Models.UserConversations
{
	public class LastMessage
	{
		public int Id { get; set; }
		public int SenderId { get; set; }
		public int ConversationId { get; set; }
		public string Body { get; set; }
		public DateTime TimeStamp { get; set; }
		public bool IsRead { get; set; }
	}
}
