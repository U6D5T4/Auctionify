using Auctionify.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Auctionify.Infrastructure.Data.Config
{
	public class RateConfiguration : IEntityTypeConfiguration<Rate>
	{
		public void Configure(EntityTypeBuilder<Rate> builder)
		{
			builder.HasOne(r => r.Receiver)
				.WithMany(r => r.ReceiverRates)
				.IsRequired(true)
				.OnDelete(DeleteBehavior.NoAction);

			builder.HasOne(r => r.Sender)
				.WithMany(r => r.SenderRates)
				.IsRequired(true)
				.OnDelete(DeleteBehavior.NoAction);

			builder.HasOne(r => r.Lot)
				.WithMany(l => l.Rates)
				.IsRequired(false)
				.OnDelete(DeleteBehavior.NoAction);

			builder.Property(r => r.RatingValue).IsRequired(true);
			builder.Property(r => r.Comment).HasMaxLength(2048).IsRequired(false);
			builder.Property(r => r.CreationDate).HasConversion(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
		}
	}
}
