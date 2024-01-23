using Auctionify.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Auctionify.Infrastructure.Data.Config
{
	public class BidConfiguration : IEntityTypeConfiguration<Bid>
	{
		public void Configure(EntityTypeBuilder<Bid> builder)
		{
			builder.Property(b => b.NewPrice).HasColumnType("decimal(28,2)").IsRequired(true);
			builder.Property(b => b.TimeStamp).IsRequired(true).HasConversion(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
			builder.Property(b => b.BidRemoved).IsRequired(true);
		}
	}
}
