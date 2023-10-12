namespace Auctionify.Application.Features.Lots.Queries.GetAllLots
{
    public class GetAllLotsResponse
    {
        public int CategoryId { get; set; }
        public int LocationId { get; set; }
        public int CurrencyId { get; set; }
        public string Title { get; set; }
        public decimal StartingPrice { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
