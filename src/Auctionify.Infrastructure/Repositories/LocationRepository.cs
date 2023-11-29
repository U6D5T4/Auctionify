using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Core.Entities;
using Auctionify.Core.Persistence.Repositories;
using Auctionify.Infrastructure.Persistence;

namespace Auctionify.Infrastructure.Repositories
{
	public class LocationRepository : EfBaseRepository<Location, ApplicationDbContext>, ILocationRepository
	{
		public LocationRepository(ApplicationDbContext context) : base(context)
		{
		}
	}
}
