using Auctionify.Application.Common.DTOs;
using Auctionify.Application.Features.Users.Commands.AddLotToWatchlist;
using Auctionify.Application.Features.Users.Commands.RemoveLotFromWatchlist;
using Auctionify.Application.Features.Users.Queries.GetByUserWatchlist;
using Auctionify.Core.Entities;
using Auctionify.Core.Persistence.Paging;
using AutoMapper;

namespace Auctionify.Application.Features.Users.Profiles
{
	public class MappingProfiles : Profile
	{
		public MappingProfiles()
		{
			CreateMap<Watchlist, AddedToWatchlistResponse>();	
			CreateMap<Watchlist, RemovedLotFromWatchlistResponse>();
			CreateMap<Lot, GetWatchlistLotsResponse>().ReverseMap();
			CreateMap<IPaginate<Lot>, GetListResponseDto<GetWatchlistLotsResponse>>().ReverseMap();
        }
    }
}
