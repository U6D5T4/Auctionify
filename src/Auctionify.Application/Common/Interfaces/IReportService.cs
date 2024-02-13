using Auctionify.Application.Common.Models.Report;

namespace Auctionify.Application.Common.Interfaces
{
	public interface ICoreReportGeneratorService
	{
		Task<byte[]> GenerateReportAsync(ReportData reportData);
	}

	public interface IPdfReportGeneratorService : ICoreReportGeneratorService { }

	public interface IXlsxReportGeneratorService : ICoreReportGeneratorService { }
}
