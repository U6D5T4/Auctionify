using Auctionify.Core.Common;

namespace Auctionify.Core.Entities
{
	public class LotStatus : BaseAuditableEntity
	{
		public string Name { get; set; }

		public virtual ICollection<Lot> Lots { get; set; }
	}
}
