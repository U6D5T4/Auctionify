using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Core.Entities;
using Auctionify.Core.Persistence.Repositories;
using Auctionify.Infrastructure.Persistence;

namespace Auctionify.Infrastructure.Repositories
{
	public class BidRepository: EfBaseRepository<Bid, ApplicationDbContext>, IBidRepository
	{
		public BidRepository(ApplicationDbContext dbContext) : base(dbContext)
		{
		}
	}
}
