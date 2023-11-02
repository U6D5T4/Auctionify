using Auctionify.Application.Features.LotStatuses.Queries.GetLotStatusesForBuyerFiltration;
using Auctionify.Core.Entities;
using AutoMapper;

namespace Auctionify.Application.Features.LotStatuses.Profiles
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<LotStatus, GetLotStatusesForBuyerFiltrationResponse>().ReverseMap();
        }
    }
}
