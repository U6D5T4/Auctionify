namespace Auctionify.Application.Features.Users.Queries.GetTransactions
{
	public class GetTransactionsUserResponse
	{
		public int? LotId { get; set; }
		public string? LotTitle { get; set; }
		public string? LotMainPhotoUrl { get; set; }
		public DateTime? TransactionDate { get; set; }
		public string? TransactionStatus { get; set; }
		public decimal? TransactionAmount { get; set; }
		public string? TransactionCurrency { get; set; }
	}
}
