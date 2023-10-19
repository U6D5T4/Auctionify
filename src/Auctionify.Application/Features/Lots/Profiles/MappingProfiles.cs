using Auctionify.Application.Features.Lots.Commands.Create;
using Auctionify.Application.Features.Lots.Commands.Delete;
using Auctionify.Application.Features.Lots.Commands.Update;
using Auctionify.Application.Features.Lots.Queries.GetAll;
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
			CreateMap<Lot, DeletedLotResponse>().ReverseMap();
			CreateMap<Lot, UpdateLotResponse>().ReverseMap();

			CreateMap<Lot, UpdateLotCommand>()
				.ForMember(l => l.Address, cd => cd.MapFrom(ul => ul.Location.Address))
				.ForMember(l => l.City, cd => cd.MapFrom(ul => ul.Location.City))
				.ForMember(l => l.Country, cd => cd.MapFrom(ul => ul.Location.Country))
				.ForMember(l => l.State, cd => cd.MapFrom(ul => ul.Location.State))
				.ReverseMap();
		}
	}
}
