using Auctionify.Core.Common;

namespace Auctionify.Core.Entities
{
	public class Bid : BaseAuditableEntity
	{
		public int BuyerId { get; set; }

		public virtual User Buyer { get; set; }

		public decimal NewPrice { get; set; }

		public DateTime TimeStamp { get; set; }

		public bool BidRemoved { get; set; }

		public virtual Lot Lot { get; set; }

		public int LotId { get; set; }
	}
}
