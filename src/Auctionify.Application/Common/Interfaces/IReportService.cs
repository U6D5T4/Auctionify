using Auctionify.Application.Common.Models.Report;
using Auctionify.Core.Entities;

namespace Auctionify.Application.Common.Interfaces
{
	public interface IReportService
	{
		Task<byte[]> GenerateReportAsync(ReportData reportData, User user, ReportType format);
	}
}
