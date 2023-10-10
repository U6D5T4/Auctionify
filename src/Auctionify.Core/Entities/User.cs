using Microsoft.AspNetCore.Identity;

namespace Auctionify.Core.Entities
{
	public class User : IdentityUser<int>
	{
		public string? FirstName { get; set; }

		public string? LastName { get; set; }

		public virtual ICollection<Lot> SellingLots { get; set; }

		public virtual ICollection<Lot> BuyingLots { get; set; }

		public virtual ICollection<Watchlist> Watchlists { get; set; }

		public virtual ICollection<ChatMessage> SenderChatMessages { get; set; }

		public virtual ICollection<ChatMessage> ReceiverChatMessages { get; set; }

		public virtual ICollection<Rate> SenderRates { get; set; }

		public virtual ICollection<Rate> ReceiverRates { get; set; }
	}
}
