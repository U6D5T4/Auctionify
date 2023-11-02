using Auctionify.Application.Common.DTOs;
using Auctionify.Application.Features.Lots.Queries.BaseQueryModels;

namespace Auctionify.Application.Features.Lots.Queries.GetAllByName
{
    public class GetAllLotsByLocationResponse : GetAllLots
    {
        public UserDto Seller { get; set; }
    }
}