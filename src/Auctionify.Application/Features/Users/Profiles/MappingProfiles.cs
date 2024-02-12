using Auctionify.Application.Common.DTOs;
using Auctionify.Application.Common.Models.Transaction;
using Auctionify.Application.Features.Users.Commands.AddBidForLot;
using Auctionify.Application.Features.Users.Commands.AddLotToWatchlist;
using Auctionify.Application.Features.Users.Commands.RemoveBid;
using Auctionify.Application.Features.Users.Commands.RemoveLotFromWatchlist;
using Auctionify.Application.Features.Users.Commands.Update;
using Auctionify.Application.Features.Users.Queries.GetAllBidsOfUserForLot;
using Auctionify.Application.Features.Users.Queries.GetBuyer;
using Auctionify.Application.Features.Users.Queries.GetBuyerAuctions;
using Auctionify.Application.Features.Users.Queries.GetById;
using Auctionify.Application.Features.Users.Queries.GetByUserWatchlist;
using Auctionify.Application.Features.Users.Queries.GetSeller;
using Auctionify.Application.Features.Users.Queries.GetTransactions;
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
			CreateMap<User, GetBuyerResponse>().ReverseMap();
			CreateMap<User, GetSellerResponse>().ReverseMap();
			CreateMap<User, UpdatedUserResponse>().ReverseMap();
			CreateMap<Watchlist, AddedToWatchlistResponse>().ReverseMap();
			CreateMap<Watchlist, RemovedLotFromWatchlistResponse>().ReverseMap();
			CreateMap<Bid, AddedBidForLotResponse>().ReverseMap();
			CreateMap<Bid, RemovedBidResponse>().ReverseMap();
			CreateMap<Bid, GetAllBidsOfUserForLotResponse>().ReverseMap();
			CreateMap<Lot, GetWatchlistLotsResponse>().ReverseMap();
			CreateMap<IPaginate<Lot>, GetListResponseDto<GetWatchlistLotsResponse>>().ReverseMap();
			CreateMap<IPaginate<Bid>, GetListResponseDto<GetAllBidsOfUserForLotResponse>>()
				.ReverseMap();
			CreateMap<Lot, GetBuyerAuctionsResponse>().ReverseMap();
			CreateMap<IPaginate<Lot>, GetListResponseDto<GetBuyerAuctionsResponse>>().ReverseMap();
			CreateMap<TransactionInfo, GetTransactionsUserResponse>().ReverseMap();
			CreateMap<IPaginate<TransactionInfo>, GetListResponseDto<GetTransactionsUserResponse>>()
				.ReverseMap();
		}
	}
}
