using Auctionify.Application.Common.DTOs;
using Auctionify.Application.Features.Rates.BaseQueyModels;

namespace Auctionify.Application.Features.Rates.Queries.GetSenderRates
{
    public class GetAllSenderRatesResponse : GetAllRates
    {
        public UserDto Sender { get; set; }
    }
}
