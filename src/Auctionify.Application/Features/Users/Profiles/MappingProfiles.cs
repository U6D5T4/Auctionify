using Auctionify.Application.Features.Users.Queries.GetById;
using Auctionify.Core.Entities;
using AutoMapper;

namespace Auctionify.Application.Features.Users.Profiles
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<User, GetByIdUserResponse>().ReverseMap();
        }
    }
}
