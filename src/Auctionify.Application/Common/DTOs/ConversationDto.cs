namespace Auctionify.Application.Common.DTOs
{
	public class ConversationDto
	{
		public int Id { get; set; }

		public int BuyerId { get; set; }

		public int SellerId { get; set; }

		public int LotId { get; set; }

		public ICollection<ChatMessageDto> ChatMessages { get; set; }
	}
}
