using Auctionify.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Auctionify.Infrastructure.Data.Config
{
	public class ChatMessageConfiguration : IEntityTypeConfiguration<ChatMessage>
	{
		public void Configure(EntityTypeBuilder<ChatMessage> builder)
		{
			builder.HasOne(s => s.Sender)
				.WithMany(u => u.ChatMessages)
				.HasForeignKey(s => s.SenderId)
				.IsRequired(true)
				.OnDelete(DeleteBehavior.NoAction);

			builder.HasOne(c => c.Conversation)
				.WithMany(c => c.ChatMessages)
				.HasForeignKey(c => c.ConversationId)
				.IsRequired(true)
				.OnDelete(DeleteBehavior.NoAction);

			builder.Property(s => s.Body).HasMaxLength(500).IsRequired(true);
			builder.Property(s => s.TimeStamp).IsRequired(true);
			builder.Property(s => s.IsRead).IsRequired(true);
		}
	}
}
