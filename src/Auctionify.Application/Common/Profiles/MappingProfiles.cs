using Auctionify.Application.Common.DTOs;
using Auctionify.Core.Entities;
using AutoMapper;

namespace Auctionify.Application.Common.Profiles
{
	public class MappingProfiles : Profile
	{
		public MappingProfiles()
		{
			CreateMap<User, UserDto>();
			CreateMap<Category, CategoryDto>();
			CreateMap<Bid, BidDto>();
			CreateMap<Currency, CurrencyDto>();
			CreateMap<LotStatus, LotStatusDto>();
			CreateMap<Location, LocationDto>();
			CreateMap<Core.Entities.File, FileDto>();
            CreateMap<Lot, LotDto>();
            CreateMap<Rate, RateDto>();
		}
	}
}
