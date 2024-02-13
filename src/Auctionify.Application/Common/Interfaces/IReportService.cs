using Auctionify.Application.Common.Models.Report;
using Auctionify.Core.Entities;

namespace Auctionify.Application.Common.Interfaces
{
	public interface ICoreReportGeneratorService
	{
		Task<byte[]> GenerateReportAsync(ReportData reportData, User user);
	}

	public interface IPdfReportGeneratorService : ICoreReportGeneratorService { }

	public interface IXlsxReportGeneratorService : ICoreReportGeneratorService { }
}
