using Auctionify.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Auctionify.Infrastructure.Data.Config
{
	public class RateConfiguration : IEntityTypeConfiguration<Rate>
	{
		public void Configure(EntityTypeBuilder<Rate> builder)
		{
			builder.HasOne(r => r.Reciever)
				.WithMany(r => r.ReceiverRates)
				.IsRequired(true)
				.OnDelete(DeleteBehavior.NoAction);

			builder.HasOne(r => r.Sender)
				.WithMany(r => r.SenderRates)
				.IsRequired(true)
				.OnDelete(DeleteBehavior.NoAction);

			builder.HasOne(r => r.Lot)
				.WithOne(l => l.Rate)
				.HasForeignKey<Rate>(r => r.LotId)
				.IsRequired(false);

			builder.Property(r => r.RatingValue).IsRequired(true);
			builder.Property(r => r.Comment).HasMaxLength(2048).IsRequired(false);
		}
	}
}
