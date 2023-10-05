using Auctionify.Core.Common;

namespace Auctionify.Core.Entities
{
	public class ChatMessage : BaseAuditableEntity
	{
		public int SellerId { get; set; }

		public int BuyerId { get; set; }
		
		public virtual User Seller { get; set; }
		
		public virtual User Buyer { get; set; }
		
		public string Message { get; set; }
		
		public virtual DateTime TimeStamp { get; set; }
		
		public int LotId { get; set; }
		
		public virtual Lot Lot { get; set; }
	}
}
