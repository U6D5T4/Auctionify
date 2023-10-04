using Auctionify.Core.Common;

namespace Auctionify.Core.Entities
{
    public class ChatMessage : BaseAuditableEntity
    {
        public int SenderId { get; set; }   
        public User Sender { get; set; }
        public int RecieverId { get; set; }
        public User Reciever { get; set; }
        public string Message { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
