using Auctionify.Application.Common.DTOs;
using Auctionify.Application.Features.Rates.BaseQueyModels;

namespace Auctionify.Application.Features.Rates.Queries.GetPublicUserRates
{
	public class GetPublicUserRatesResponse : GetAllRates
	{
		public UserDto Sender { get; set; }
	}
}
