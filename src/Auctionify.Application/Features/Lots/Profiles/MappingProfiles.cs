using Auctionify.Application.Features.Lots.Queries.GetAllLots;
using Auctionify.Core.Entities;
using AutoMapper;

namespace Auctionify.Application.Features.Lots.Profiles
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles() 
        {
            CreateMap<Lot, GetAllLotsResponse>().ReverseMap();
        }
    }
}
