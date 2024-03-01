using Auctionify.Core.Enums;

namespace Auctionify.Application.Features.Reports.Query.GetCreatedLotsCount
{
	public class GetCreatedLotsCountResponse
	{
		public AnalyticReportPeriod Period { get; set; }

		public IList<CreatedLotsDay> Data { get; set; }
	}

	public class CreatedLotsDay
	{
		public DateTime Date { get; set; }

		public long Count { get; set; }
	}
}
