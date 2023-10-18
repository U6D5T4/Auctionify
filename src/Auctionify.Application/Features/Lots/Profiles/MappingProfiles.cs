﻿using Auctionify.Application.Features.Lots.Commands.Create;
using Auctionify.Application.Features.Lots.Commands.Delete;
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
		}
	}
}
