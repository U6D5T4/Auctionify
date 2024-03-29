﻿using Auctionify.Core.Common;

namespace Auctionify.Core.Entities
{
	public class Subscription : BaseAuditableEntity
	{
		public int UserId { get; set; }

		public virtual User User { get; set; }

		public int SubscriptionTypeId { get; set; }

		public virtual SubscriptionType SubscriptionType { get; set; }

		public bool IsActive { get; set; }

		public DateTime StartDate { get; set; }

		public DateTime? EndDate { get; set; }
	}
}
