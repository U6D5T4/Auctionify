using Auctionify.Core.Entities;

namespace Auctionify.Application.Common.Models.Report
{
	public class ReportData
	{
		public int TotalSoldItems { get; set; }
		public decimal TotalCostOfSoldItems { get; set; }
		public List<MonthlyReportData> MonthlyReports { get; set; } = new List<MonthlyReportData>();
	}
}
