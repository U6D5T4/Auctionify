using Auctionify.Application.Features.Lots.Queries.BaseQueryModels;

namespace Auctionify.Application.Features.Users.Queries.GetByUserWatchlist
{
	public class GetWatchlistLotsResponse : GetAllLots
	{
		public bool IsInWatchlist { get; set; }
		public int WatchlistId { get; set; }
	}
}
