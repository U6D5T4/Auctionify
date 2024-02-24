using Auctionify.Core.Common;

namespace Auctionify.Core.Entities
{
	public class Location : BaseAuditableEntity
	{
		public string City { get; set; }

		public string State { get; set; }

		public string Country { get; set; }

		public string Address { get; set; }

		public string Latitude { get; set; }

		public string Longitude { get; set; }

		public virtual Lot Lot { get; set; }
	}
}
