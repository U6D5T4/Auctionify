using Auctionify.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Auctionify.Infrastructure.Data.Config
{
	public class LotConfiguration : IEntityTypeConfiguration<Lot>
	{
		public void Configure(EntityTypeBuilder<Lot> builder)
		{
			builder.HasOne(l => l.Seller)
				.WithMany(u => u.SellingLots)
				.IsRequired(true)
				.OnDelete(DeleteBehavior.NoAction);

			builder.HasOne(l => l.Buyer)
				.WithMany(u => u.BuyingLots)
				.IsRequired(false)
				.OnDelete(DeleteBehavior.NoAction);

			builder.HasOne(l => l.Category)
				.WithMany(c => c.Lots)
				.IsRequired(true);

			builder.HasOne(l => l.LotStatus)
				.WithMany(ls => ls.Lots)
				.IsRequired(true);

			builder.HasOne(l => l.Location)
				.WithOne(l => l.Lot)
				.IsRequired(true);

			builder.HasOne(l => l.Currency)
				.WithMany(c => c.Lots)
				.IsRequired(true);

			builder.HasMany(l => l.Bids)
				.WithOne(b => b.Lot)
				.IsRequired(false)
				.OnDelete(DeleteBehavior.NoAction);

			builder.Property(l => l.Title).HasMaxLength(100).IsRequired(false);
			builder.Property(l => l.Description).HasMaxLength(500).IsRequired(false);
			builder.Property(l => l.StartingPrice).HasColumnType("decimal(7,2)").IsRequired(true);
			builder.Property(l => l.StartDate).IsRequired(true);
			builder.Property(l => l.EndDate).IsRequired(true);


		}
	}
}
