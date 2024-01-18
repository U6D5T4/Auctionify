using Auctionify.Application.Common.DTOs;

namespace Auctionify.Application.Features.Chats.Queries.GetAllChatMessages
{
	public class GetAllChatMessagesResponse
	{
		public List<ChatMessageDto> ChatMessages { get; set; }
	}
}