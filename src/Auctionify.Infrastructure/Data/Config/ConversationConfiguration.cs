using Auctionify.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Auctionify.Infrastructure.Data.Config
{
	public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
	{
		public void Configure(EntityTypeBuilder<Conversation> builder)
		{
			builder
				.HasOne(s => s.Seller)
				.WithMany(s => s.SellerConversations)
				.IsRequired(true)
				.OnDelete(DeleteBehavior.NoAction);

			builder
				.HasOne(s => s.Buyer)
				.WithMany(s => s.BuyerConversations)
				.IsRequired(true)
				.OnDelete(DeleteBehavior.NoAction);
		}
	}
}
