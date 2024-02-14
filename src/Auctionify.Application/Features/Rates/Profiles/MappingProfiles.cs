using Auctionify.Application.Common.DTOs;
using Auctionify.Application.Features.Rates.Commands.AddRateToBuyer;
using Auctionify.Application.Features.Rates.Commands.AddRateToSeller;
using Auctionify.Application.Features.Rates.Queries.GetReceiverRates;
using Auctionify.Application.Features.Rates.Queries.GetSenderRate;
using Auctionify.Application.Features.Rates.Queries.GetSenderRates;
using Auctionify.Core.Entities;
using Auctionify.Core.Persistence.Paging;
using AutoMapper;

namespace Auctionify.Application.Features.Rates.Profiles
{
	public class MappingProfiles : Profile
	{
		public MappingProfiles()
		{
			CreateMap<Rate, GetAllSenderRatesResponse>().ReverseMap();
			CreateMap<Rate, GetAllReceiverRatesResponse>().ReverseMap();
			CreateMap<IPaginate<Rate>, GetListResponseDto<GetAllSenderRatesResponse>>().ReverseMap();
			CreateMap<IPaginate<Rate>, GetListResponseDto<GetAllReceiverRatesResponse>>().ReverseMap();
			CreateMap<Rate, AddRateToBuyerResponse>().ReverseMap();
			CreateMap<Rate, AddRateToSellerResponse>().ReverseMap();
			CreateMap<Rate, GetSenderRateResponse>().ReverseMap();
		}
	}
}
