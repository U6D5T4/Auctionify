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
				.HasForeignKey(l => l.SellerId)
				.IsRequired(true)
				.OnDelete(DeleteBehavior.NoAction);

			builder.HasOne(l => l.Buyer)
				.WithMany(u => u.BuyingLots)
				.HasForeignKey(l => l.BuyerId)
				.IsRequired(false)
				.OnDelete(DeleteBehavior.NoAction);

			builder.HasOne(l => l.Category)
				.WithMany(c => c.Lots)
				.HasForeignKey(l => l.CategoryId)
				.IsRequired(false);

			builder.HasOne(l => l.LotStatus)
				.WithMany(ls => ls.Lots)
				.HasForeignKey(l => l.LotStatusId)
				.IsRequired(true);

			builder.HasOne(l => l.Location)
				.WithOne(l => l.Lot)
				.IsRequired(true);

			builder.HasOne(l => l.Currency)
				.WithMany(c => c.Lots)
				.HasForeignKey(l => l.CurrencyId)
				.IsRequired(false);

			builder.HasMany(l => l.Bids)
				.WithOne(b => b.Lot)
				.IsRequired(false)
				.OnDelete(DeleteBehavior.NoAction);

			builder.Property(l => l.Title).HasMaxLength(100).IsRequired(false);
			builder.Property(l => l.Description).HasMaxLength(500).IsRequired(false);
			builder.Property(l => l.StartingPrice).HasColumnType("decimal(28,2)").IsRequired(false);
			builder.Property(l => l.StartDate).IsRequired(true);
			builder.Property(l => l.EndDate).IsRequired(true);


		}
	}
}
