using Auctionify.Application.Features.Categories.Queries.GetAll;
using Auctionify.Core.Entities;
using Auctionify.Core.Persistence.Paging;
using AutoMapper;

namespace Auctionify.Application.Features.Categories.Profiles
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles() {
            CreateMap<Category, GetAllCateogoriesResponse>().ReverseMap();
        }
    }
}
