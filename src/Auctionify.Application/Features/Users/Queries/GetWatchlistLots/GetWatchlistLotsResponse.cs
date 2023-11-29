using Auctionify.Application.Features.Lots.Queries.BaseQueryModels;

namespace Auctionify.Application.Features.Users.Queries.GetByUserWatchlist
{
	public class GetWatchlistLotsResponse : GetAllLots
	{
		public int BidCount { get; set; }
	}
}
