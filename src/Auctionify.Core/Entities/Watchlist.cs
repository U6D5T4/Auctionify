using Auctionify.Core.Common;

namespace Auctionify.Core.Entities
{
    public class Watchlist : BaseAuditableEntity
    {
        public int UserId { get; set; }

        public User User { get; set; }

        public ICollection<Lot> Lots { get; set; }

    }
}
