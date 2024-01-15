using Auctionify.Core.Common;

namespace Auctionify.Core.Entities
{
	public class ChatMessage : BaseAuditableEntity
	{
		public int SenderId { get; set; }

		public virtual User Sender { get; set; }

		public int ConversationId { get; set; }

		public virtual Conversation Conversation { get; set; }

		public string Body { get; set; }

		public bool IsRead { get; set; }

		public DateTime TimeStamp { get; set; }
	}
}
