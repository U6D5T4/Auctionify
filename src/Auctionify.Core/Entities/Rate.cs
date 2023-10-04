using Auctionify.Core.Common;

namespace Auctionify.Core.Entities
{
    public class Rate : BaseAuditableEntity
    {
        public int RecieverId { get; set; }

        public virtual User Reciever { get; set; }

        public int SenderId { get; set; }

        public virtual User Sender { get; set; }

        public int RatingValue { get; set; }

        public string Comment { get; set; }
    }
}
