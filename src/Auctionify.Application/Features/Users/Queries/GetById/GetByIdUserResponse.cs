namespace Auctionify.Application.Features.Users.Queries.GetById
{
	public class GetByIdUserResponse
	{
		public int Id { get; set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }

		public string Email { get; set; }

		public string PhoneNumber { get; set; }

		public string AboutMe { get; set; }

		public string ProfilePictureUrl { get; set; }

		public double AverageRate { get; set; }

		public int RatesCount { get; set; }

		public Dictionary<byte, int> StarCounts { get; set; } = new Dictionary<byte, int>();

		public bool IsDeleted { get; set; }
	}
}

