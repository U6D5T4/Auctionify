using Auctionify.Application.Features.Lots.Queries.BaseQueryModels;

namespace Auctionify.Application.Features.Lots.Queries.GetAll
{
	public class GetAllLotsResponse : GetAllLots
	{
		public bool IsInWatchlist { get; set; }
		public int BidCount { get; set; }
	}
}