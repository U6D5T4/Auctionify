using Auctionify.Application.Common.DTOs;

namespace Auctionify.Application.Features.Users.Queries.GetBuyer
{
	public class GetBuyerResponse
	{
		public string FirstName { get; set; }

		public string LastName { get; set; }

		public string Email { get; set; }

		public string PhoneNumber { get; set; }

		public string AboutMe { get; set; }

		public string ProfilePictureUrl { get; set; }

		public ICollection<RateDto> SenderRates { get; set; }

		public ICollection<RateDto> ReceiverRates { get; set; }

		public double AverageRate { get; set; }

		public int RatesCount { get; set; }
	}
}
