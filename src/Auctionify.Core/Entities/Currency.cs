using Auctionify.Core.Common;

namespace Auctionify.Core.Entities
{
	public class Currency : BaseAuditableEntity
	{
		public string Code { get; set; }

		public virtual ICollection<Lot> Lots { get; set; }
	}
}
