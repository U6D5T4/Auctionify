namespace Auctionify.Application.Features.Rates.BaseQueyModels
{
	public class GetAllRates
	{
		public int Id { get; set; }

		public byte RatingValue { get; set; }

		public string Comment { get; set; }

		public DateTime CreationDate { get; set; }
	}
}
