using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Auctionify.Infrastructure.Data.Config
{
    public class FileConfiguration : IEntityTypeConfiguration<Core.Entities.File>
    {
        public void Configure(EntityTypeBuilder<Core.Entities.File> builder)
        {
            builder.Property(f => f.FileName).HasMaxLength(150).IsRequired(true);
            builder.Property(f => f.Path).HasMaxLength(256).IsRequired(true);
            //builder.HasOne(f => f.Lot).WithMany(l => l.)
        }
    }
}
