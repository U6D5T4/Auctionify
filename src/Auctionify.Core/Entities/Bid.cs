using Auctionify.Core.Common;

namespace Auctionify.Core.Entities
{
    public class Bid : BaseAuditableEntity
    {
        public int BuyerId { get; set; }

        public User Buyer { get; set; }

        public decimal NewPrice { get; set; }

        public DateTime TimeStamp { get; set; }
    }
}
