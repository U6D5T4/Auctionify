using Auctionify.Application.Common.DTOs;
using Auctionify.Application.Features.Rates.BaseQueyModels;

namespace Auctionify.Application.Features.Rates.Queries.GetReceiverRates
{
    public class GetAllReceiverRatesResponse : GetAllRates
    {
        public UserDto Reciever { get; set; }
    }
}
