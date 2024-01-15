using Auctionify.Core.Common;

namespace Auctionify.Core.Entities
{
	public class Conversation : BaseAuditableEntity
	{
		public int BuyerId { get; set; }

		public int SellerId { get; set; }

		public virtual User Buyer { get; set; }

		public virtual User Seller { get; set; }

		public int LotId { get; set; }

		public virtual Lot Lot { get; set; }

		public virtual ICollection<ChatMessage> ChatMessages { get; set; }
	}
}
