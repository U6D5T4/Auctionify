using Auctionify.Application.Common.Models.Requests;
using Auctionify.Core.Entities;
using Auctionify.Core.Enums;

namespace Auctionify.UnitTests.GetTransactionsTests
{
	public static class TransactionTestData
	{
		public static IEnumerable<object[]> GetTestData()
		{
			var lots = new List<Lot>
			{
				new()
				{
					Id = 2,
					Title = "Sample Lot 2",
					Currency = new Currency { Code = "USD" },
					LotStatus = new LotStatus { Name = AuctionStatus.NotSold.ToString() }
				},
				new()
				{
					Id = 15,
					Title = "Lexus SC400",
					Currency = new Currency { Code = "USD" },
					LotStatus = new LotStatus { Name = AuctionStatus.Sold.ToString() }
				},
				new()
				{
					Id = 14,
					Title = "Ancient Car",
					Currency = new Currency { Code = "USD" },
					LotStatus = new LotStatus { Name = AuctionStatus.NotSold.ToString() }
				},
			};

			var highestBid = new Bid
			{
				Id = 1,
				LotId = 15,
				NewPrice = 4000,
				BidRemoved = false,
			};

			var pageRequest = new PageRequest { PageIndex = 0, PageSize = 10 };

			yield return new object[] { lots, highestBid, pageRequest };
		}
	}
}
