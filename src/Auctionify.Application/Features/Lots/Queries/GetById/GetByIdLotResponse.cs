using Auctionify.Core.Entities;

namespace Auctionify.Application.Features.Lots.Queries.GetById
{
	public class GetByIdLotResponse
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public decimal StartingPrice { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public Category Category { get; set; }
		public LotStatus LotStatus { get; set; }
		public Location Location { get; set; }
		public Currency Currency { get; set; }
		public ICollection<Bid> Bids { get; set; }
	}
}
