namespace Auctionify.Application.Common.Models.Transaction
{
	public class TransactionInfo
	{
		public string? LotMainPhotoUrl { get; set; }
		public string? LotTitle { get; set; }
		public DateTime? TransactionDate { get; set; }
		public string? TransactionStatus { get; set; }
		public decimal? TransactionAmount { get; set; }
		public string? TransactionCurrency { get; set; }
	}
}
