using Auctionify.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Auctionify.Infrastructure.Data.Config
{
	public class ChatMessageConfiguration : IEntityTypeConfiguration<ChatMessage>
	{
		public void Configure(EntityTypeBuilder<ChatMessage> builder)
		{
			builder.HasOne(s => s.Seller)
				.WithMany(s => s.SenderChatMessages)
				.IsRequired(true)
				.OnDelete(DeleteBehavior.NoAction);

			builder.HasOne(s => s.Buyer)
				.WithMany(s => s.ReceiverChatMessages)
				.IsRequired(true)
				.OnDelete(DeleteBehavior.NoAction);

			builder.Property(s => s.Message).HasMaxLength(500).IsRequired(true);
			builder.Property(s => s.TimeStamp).IsRequired(true);
		}
	}
}
