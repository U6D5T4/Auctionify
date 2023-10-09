using Auctionify.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Auctionify.Infrastructure.Data.Config
{
    public class LotStatusConfiguration : IEntityTypeConfiguration<LotStatus>
    {
        public void Configure(EntityTypeBuilder<LotStatus> builder)
        {
            builder.Property(s => s.Name).HasMaxLength(50).IsRequired(true);
        }
    }
}
