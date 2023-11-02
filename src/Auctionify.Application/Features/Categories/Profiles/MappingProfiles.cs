using Auctionify.Application.Features.Categories.Queries.GetAll;
using Auctionify.Core.Entities;
using AutoMapper;

namespace Auctionify.Application.Features.Categories.Profiles
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles() {
            CreateMap<Category, GetAllCategoriesResponse>().ReverseMap();
        }
    }
}
