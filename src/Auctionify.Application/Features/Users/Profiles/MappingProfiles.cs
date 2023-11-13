using Auctionify.Application.Common.DTOs;
using Auctionify.Application.Features.Users.Commands.AddBidForLot;
using Auctionify.Application.Features.Users.Commands.AddLotToWatchlist;
using Auctionify.Application.Features.Users.Commands.RemoveLotFromWatchlist;
using Auctionify.Application.Features.Users.Queries.GetById;
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
			CreateMap<User, GetByIdUserResponse>().ReverseMap();
			CreateMap<Watchlist, AddedToWatchlistResponse>().ReverseMap();
			CreateMap<Watchlist, RemovedLotFromWatchlistResponse>().ReverseMap();
			CreateMap<Bid, AddedBidForLotResponse>().ReverseMap();
			CreateMap<Lot, GetWatchlistLotsResponse>().ReverseMap();
			CreateMap<IPaginate<Lot>, GetListResponseDto<GetWatchlistLotsResponse>>().ReverseMap();
		}
	}
}
