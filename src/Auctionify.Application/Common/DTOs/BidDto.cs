using Auctionify.Core.Entities;

namespace Auctionify.Application.Common.DTOs
{
    public class BidDto
    {
        public int Id { get; set; }

        public int BuyerId { get; set; }

        public virtual User Buyer { get; set; }

        public decimal NewPrice { get; set; }

        public virtual DateTime TimeStamp { get; set; }
    }
}
