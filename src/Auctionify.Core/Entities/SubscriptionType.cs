using Auctionify.Core.Common;

namespace Auctionify.Core.Entities
{
	public class SubscriptionType : BaseAuditableEntity
	{
		public string Name { get; set; }

		public virtual ICollection<Subscription> Subscriptions { get; set; }
	}
}
