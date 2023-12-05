namespace Auctionify.Application.Features.Users.Queries.GetSeller
{
	public class GetSellerResponse
	{
		public string FirstName { get; set; }

		public string LastName { get; set; }

		public string Email { get; set; }

		public string PhoneNumber { get; set; }

		public string AboutMe { get; set; }

		public string ProfilePictureUrl { get; set; }

		public int CreatedLotsCount { get; set; }

		public int FinishedLotsCount { get; set; }
	}
}
