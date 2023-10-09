using Auctionify.Core.Common;

namespace Auctionify.Core.Entities
{
	public class Watchlist : BaseAuditableEntity
	{
		public int UserId { get; set; }

		public int LotId { get; set; }

		public virtual User User { get; set; }

		public virtual Lot Lot { get; set; }
	}
}
