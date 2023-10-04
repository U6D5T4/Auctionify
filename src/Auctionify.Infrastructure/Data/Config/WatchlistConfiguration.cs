using Auctionify.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Auctionify.Infrastructure.Data.Config
{
    public class WatchlistConfiguration : IEntityTypeConfiguration<Watchlist>
    {
        public void Configure(EntityTypeBuilder<Watchlist> builder)
        {
            builder.HasMany(w => w.Lots);
        }
    }
}
