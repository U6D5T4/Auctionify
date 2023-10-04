using Auctionify.Core.Common;

namespace Auctionify.Core.Entities
{
    public class ChatMessage : BaseAuditableEntity
    {
        public int SenderId { get; set; }   
        public virtual User Sender { get; set; }
        public int RecieverId { get; set; }
        public virtual User Reciever { get; set; }
        public string Message { get; set; }
        public virtual DateTime TimeStamp { get; set; }
    }
}
