namespace Auctionify.Core.Entities
{
	public class MonthlyReportData
	{
		public string ReportMonth { get; set; }
		public int SoldLotsCount { get; set; }
		public decimal SoldLotsTotalCost { get; set; }
		public int CreatedLotsCount { get; set; }
	}
}
