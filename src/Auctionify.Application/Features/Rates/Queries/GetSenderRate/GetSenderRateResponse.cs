using Auctionify.Application.Common.DTOs;
using Auctionify.Application.Features.Rates.BaseQueyModels;

namespace Auctionify.Application.Features.Rates.Queries.GetSenderRate
{
	public class GetSenderRateResponse : GetAllRates
	{
		public UserDto Sender { get; set; }
	}
}
