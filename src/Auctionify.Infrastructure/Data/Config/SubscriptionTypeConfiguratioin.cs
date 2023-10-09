using Auctionify.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Auctionify.Infrastructure.Data.Config
{
	public class SubscriptionTypeConfiguratioin : IEntityTypeConfiguration<SubscriptionType>
	{
		public void Configure(EntityTypeBuilder<SubscriptionType> builder)
		{
			builder.Property(s => s.Name).HasMaxLength(50).IsRequired(true);
		}
	}
}
