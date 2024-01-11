using Auctionify.Application.Features.Lots.Queries.BaseQueryModels;

namespace Auctionify.Application.Features.Users.Queries.GetTransactions
{
	public class GetTransactionsUserResponse : GetAllLots
	{
		public DateTime TransactionDate { get; set; }
		public string TransactionStatus { get; set; }
		public decimal TransactionAmount { get; set; }
	}
}
