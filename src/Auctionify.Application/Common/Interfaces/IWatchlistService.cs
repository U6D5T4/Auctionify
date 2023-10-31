namespace Auctionify.Application.Common.Interfaces
{
	public interface IWatchlistService
	{
		public Task<bool> IsLotInUserWatchlist(int lotId, int userId, CancellationToken cancellationToken);
	}
}
