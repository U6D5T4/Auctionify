using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Core.Entities;
using Auctionify.Core.Persistence.Repositories;
using Auctionify.Infrastructure.Persistence;

namespace Auctionify.Infrastructure.Repositories
{
	public class LotRepository : EfBaseRepository<Lot, ApplicationDbContext>, ILotRepository
	{
		public LotRepository(ApplicationDbContext dbContext) : base(dbContext)
		{
		}
	}
}
