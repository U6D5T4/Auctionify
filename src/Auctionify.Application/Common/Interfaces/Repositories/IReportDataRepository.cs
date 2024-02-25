using Auctionify.Application.Common.Models.Report;

namespace Auctionify.Application.Common.Interfaces.Repositories
{
	public interface IReportDataRepository
	{
		Task<ReportData> GetReportDataAsync(ReportRequest request);
	}
}
