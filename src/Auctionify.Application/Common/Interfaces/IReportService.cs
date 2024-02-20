using Auctionify.Application.Common.Models.Report;
using Auctionify.Core.Entities;

namespace Auctionify.Application.Common.Interfaces
{
	public interface IPdfReportGeneratorService 
	{
		Task<byte[]> GenerateReportAsync(ReportData reportData, User user);
	}

	public interface IXlsxReportGeneratorService 
	{
		Task<byte[]> GenerateReportAsync(ReportData reportData, User user);
	}
}
