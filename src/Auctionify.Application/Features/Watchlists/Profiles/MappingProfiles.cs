using Auctionify.Application.Common.DTOs;
using Auctionify.Application.Features.Watchlists.Commands.AddLot;
using Auctionify.Application.Features.Watchlists.Commands.RemoveLot;
using Auctionify.Application.Features.Watchlists.Queries.GetByUserId;
using Auctionify.Core.Entities;
using Auctionify.Core.Persistence.Paging;
using AutoMapper;

namespace Auctionify.Application.Features.Watchlists.Profiles
{
	public class MappingProfiles : Profile
	{
		public MappingProfiles()
		{
			CreateMap<Watchlist, AddedToWatchlistResponse>();	
			CreateMap<Watchlist, RemovedLotFromWatchlistResponse>();
			//CreateMap<Watchlist, GetByUserIdWatchlistResponse>();

			//CreateMap<IPaginate<Watchlist>, GetListResponseDto<GetByUserIdWatchlistResponse>>().ReverseMap();

			CreateMap<Lot, GetByUserIdWatchlistResponse>().ReverseMap();
			CreateMap<IPaginate<Lot>, GetListResponseDto<GetByUserIdWatchlistResponse>>().ReverseMap();
        }
    }
}
