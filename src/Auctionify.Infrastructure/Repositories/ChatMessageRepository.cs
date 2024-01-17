using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Core.Entities;
using Auctionify.Core.Persistence.Repositories;
using Auctionify.Infrastructure.Persistence;

namespace Auctionify.Infrastructure.Repositories
{
	public class ChatMessageRepository
		: EfBaseRepository<ChatMessage, ApplicationDbContext>,
			IChatMessageRepository
	{
		public ChatMessageRepository(ApplicationDbContext dbContext)
			: base(dbContext) { }
	}
}
