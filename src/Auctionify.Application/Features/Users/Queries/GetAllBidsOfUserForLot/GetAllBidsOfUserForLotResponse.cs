namespace Auctionify.Application.Features.Users.Queries.GetAllBidsOfUserForLot
{
	public class GetAllBidsOfUserForLotResponse
	{
		public int Id { get; set; }

		public decimal NewPrice { get; set; }

		public DateTime TimeStamp { get; set; }

		public string Currency { get; set; }

		public bool BidRemoved { get; set; }
	}
}
