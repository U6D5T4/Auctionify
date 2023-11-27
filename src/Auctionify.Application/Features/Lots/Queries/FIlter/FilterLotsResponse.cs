using Auctionify.Application.Features.Lots.Queries.BaseQueryModels;

namespace Auctionify.Application.Features.Lots.Queries.FIlter
{
	public class FilterLotsResponse : GetAllLots
	{
		public bool IsInWatchlist { get; set; }
	}
}
