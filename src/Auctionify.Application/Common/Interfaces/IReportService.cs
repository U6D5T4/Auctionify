using Auctionify.Application.Common.Models.Report;
using Auctionify.Core.Entities;

namespace Auctionify.Application.Common.Interfaces
{
	public interface IReportService
	{
		byte[] GenerateReport(ReportData reportData, User user, ReportType format);
	}
}
