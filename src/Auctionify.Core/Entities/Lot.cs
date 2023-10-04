using Auctionify.Core.Common;

namespace Auctionify.Core.Entities
{
    public class Lot : BaseAuditableEntity
    {
        public int SellerId { get; set; }

        public User Seller { get; set; }

        public int BuyerId { get; set; }

        public User? Buyer { get; set; }

        public int CategoryId { get; set; }

        public Category? Category { get; set; }

        public int LotStatusId { get; set; }

        public LotStatus LotStatus { get; set; }

        public int LocationId { get; set; }

        public Location? Location { get; set; }

        public int CurrencyId { get; set; }

        public Currency? Currency { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public decimal? StartingPrice { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
