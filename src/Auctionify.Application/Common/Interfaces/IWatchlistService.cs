namespace Auctionify.Application.Common.Interfaces
{
	public interface IWatchlistService
	{
		Task<bool> IsLotInUserWatchlist(int lotId, int userId, CancellationToken cancellationToken);
	}
}
