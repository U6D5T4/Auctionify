using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Core.Entities;
using Auctionify.Core.Persistence.Repositories;
using Auctionify.Infrastructure.Persistence;

namespace Auctionify.Infrastructure.Repositories
{
	public class WatchlistRepository: EfBaseRepository<Watchlist, ApplicationDbContext>, IWatchlistRepository
	{
		public WatchlistRepository(ApplicationDbContext context) : base(context)
		{
		}
	}
}
