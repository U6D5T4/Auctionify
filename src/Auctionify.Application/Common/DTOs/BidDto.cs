using Auctionify.Core.Entities;

namespace Auctionify.Application.Common.DTOs
{
    public class BidDto
    {
        public int Id { get; set; }

        public int BuyerId { get; set; }

        public virtual UserDto Buyer { get; set; }

        public decimal NewPrice { get; set; }

        public virtual DateTime TimeStamp { get; set; }
    }
}
