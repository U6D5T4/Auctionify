using Auctionify.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Auctionify.Infrastructure.Data.Config
{
	public class WatchlistConfiguration : IEntityTypeConfiguration<Watchlist>
	{
		public void Configure(EntityTypeBuilder<Watchlist> builder)
		{
			builder.HasOne(w => w.User)
				.WithMany(u => u.Watchlists)
				.IsRequired(true);

			builder.HasOne(w => w.Lot)
				.WithMany(l => l.Watchlists)
				.IsRequired(true);

			builder.Property(w => w.UserId).IsRequired(true);
			builder.Property(w => w.LotId).IsRequired(true);
		}
	}
}
