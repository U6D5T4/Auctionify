using Auctionify.Application.Features.Currencies.Queries.GetAll;
using Auctionify.Core.Entities;
using AutoMapper;

namespace Auctionify.Application.Features.Currencies.Profiles
{
	public class MappingProfiles : Profile
	{
		public MappingProfiles()
		{
			CreateMap<Currency, GetAllCurrenciesResponse>().ReverseMap();
		}
	}
}
