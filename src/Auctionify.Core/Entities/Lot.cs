using Auctionify.Core.Common;

namespace Auctionify.Core.Entities
{
	public class Lot : BaseAuditableEntity
	{
		public int SellerId { get; set; }

		public virtual User Seller { get; set; }

		public int BuyerId { get; set; }

		public virtual User? Buyer { get; set; }

		public int CategoryId { get; set; }

		public virtual Category? Category { get; set; }

		public int LotStatusId { get; set; }

		public virtual LotStatus LotStatus { get; set; }

		public int LocationId { get; set; }

		public virtual Location? Location { get; set; }

		public int CurrencyId { get; set; }

		public virtual Currency? Currency { get; set; }

		public string? Title { get; set; }

		public string? Description { get; set; }

		public decimal? StartingPrice { get; set; }

		public virtual DateTime StartDate { get; set; }

		public virtual DateTime EndDate { get; set; }

		public virtual ICollection<Watchlist> Watchlists { get; set; }

		public virtual Rate? Rate { get; set; }

		public int? RateId { get; set; }

		public virtual ICollection<Bid> Bids { get; set; }
	}
}
