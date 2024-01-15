using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Core.Entities;
using Auctionify.Core.Persistence.Repositories;
using Auctionify.Infrastructure.Persistence;

namespace Auctionify.Infrastructure.Repositories
{
	public class RateRepository : EfBaseRepository<Rate, ApplicationDbContext>, IRateRepository
	{
		public RateRepository(ApplicationDbContext dbContext) : base(dbContext)
		{
		}
	}
}
