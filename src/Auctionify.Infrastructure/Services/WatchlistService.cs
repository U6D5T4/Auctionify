using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;

namespace Auctionify.Infrastructure.Services
{
	public class WatchlistService : IWatchlistService
	{
		private readonly IWatchlistRepository _watchlistRepository;

		public WatchlistService(IWatchlistRepository watchlistRepository)
		{
			_watchlistRepository = watchlistRepository;
		}

		public async Task<bool> IsLotInUserWatchlist(int lotId, int userId, CancellationToken cancellationToken)
		{
			bool isLotInUserWatchlist =
				await _watchlistRepository.GetAsync(
					predicate: x => x.LotId == lotId && x.UserId == userId,
					enableTracking: false,
					cancellationToken: cancellationToken
				) != null;

			return isLotInUserWatchlist;
		}
	}
}
