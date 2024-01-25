namespace Auctionify.Application.Features.Chats.Commands.MarkChatMessageAsRead
{
	public class MarkedChatMessageAsReadResponse
	{
		public int ChatMessageId { get; set; }
		public bool Success { get; set; }
		public string Message { get; set; }
	}
}
