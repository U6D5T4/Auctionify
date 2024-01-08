using Auctionify.Application.Features.Lots.Queries.BaseQueryModels;

namespace Auctionify.Application.Features.Users.Queries.GetBuyerAuctions
{
	public class GetBuyerAuctionsResponse : GetAllLots
	{
		public bool IsInWatchlist { get; set; }
	}
}
