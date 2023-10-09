using Auctionify.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Auctionify.Infrastructure.Data.Config
{
	public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
	{
		public void Configure(EntityTypeBuilder<Subscription> builder)
		{
			builder.HasOne(s => s.SubscriptionType)
				.WithMany(s => s.Subscriptions)
				.IsRequired(true);

			builder.Property(s => s.UserId).IsRequired(true);
			builder.Property(s => s.IsActive).IsRequired(true);
			builder.Property(s => s.StartDate).IsRequired(true);
			builder.Property(s => s.EndDate);

		}
	}
}