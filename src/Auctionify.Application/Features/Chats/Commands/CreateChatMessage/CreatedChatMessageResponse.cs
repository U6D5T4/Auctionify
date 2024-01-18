namespace Auctionify.Application.Features.Chats.Commands.CreateChatMessage
{
	public class CreatedChatMessageResponse
	{
		public int Id { get; set; }

		public int SenderId { get; set; }

		public int ConversationId { get; set; }

		public string Body { get; set; }

		public bool IsRead { get; set; }

		public DateTime TimeStamp { get; set; }
	}
}
