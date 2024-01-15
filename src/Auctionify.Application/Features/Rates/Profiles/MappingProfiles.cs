using Auctionify.Application.Features.Rates.Commands.AddRateToBuyer;
using Auctionify.Application.Features.Rates.Commands.AddRateToSeller;
using Auctionify.Core.Entities;
using AutoMapper;

namespace Auctionify.Application.Features.Rates.Profiles
{
	public class MappingProfiles : Profile
	{
		public MappingProfiles() 
		{
			CreateMap<Rate, AddRateToBuyerResponse>().ReverseMap();
			CreateMap<Rate, AddRateToSellerResponse>().ReverseMap();
		}
	}
}
