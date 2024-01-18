namespace Auctionify.Application.Common.DTOs
{
	public class ChatMessageDto
	{
		public int Id { get; set; }

		public int SenderId { get; set; }

		public int ConversationId { get; set; }

		public string Body { get; set; }

		public bool IsRead { get; set; }

		public DateTime TimeStamp { get; set; }
	}
}
