using Auctionify.Application.Features.Lots.Commands.Create;
using Auctionify.Application.Features.Lots.Queries.GetAllLots;
using Auctionify.Application.Features.Lots.Queries.GetById;
using Auctionify.Core.Entities;
using AutoMapper;

namespace Auctionify.Application.Features.Lots.Profiles
{
	public class MappingProfiles : Profile
	{
		public MappingProfiles() 
		{
            CreateMap<Lot, GetAllLotsResponse>().ReverseMap();
            CreateMap<Lot, CreatedLotResponse>().ReverseMap();
			CreateMap<Lot, GetByIdLotResponse>().ReverseMap();
		}
	}
}
