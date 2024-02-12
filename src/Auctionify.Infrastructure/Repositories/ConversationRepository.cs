using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Core.Entities;
using Auctionify.Core.Persistence.Repositories;
using Auctionify.Infrastructure.Persistence;

namespace Auctionify.Infrastructure.Repositories
{
	public class ConversationRepository
		: EfBaseRepository<Conversation, ApplicationDbContext>,
			IConversationRepository
	{
		public ConversationRepository(ApplicationDbContext dbContext)
			: base(dbContext) { }
	}
}
