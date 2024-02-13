using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Models.Report;
using ClosedXML.Excel;

namespace Auctionify.Infrastructure.Services.ReportGeneratingServices
{
	public class XlsxReportGeneratorService : IXlsxReportGeneratorService
	{
		public Task<byte[]> GenerateReportAsync(ReportData reportData)
		{
			using (var workbook = new XLWorkbook())
			{
				var worksheet = workbook.Worksheets.Add("Report");

				worksheet.Cell("A1").Value = "Total Items";
				worksheet.Cell("B1").Value = reportData.TotalItemsCount;

				worksheet.Cell("A2").Value = "Sold Items";
				worksheet.Cell("B2").Value = reportData.SoldItemsCount;

				worksheet.Cell("A3").Value = "Total Sold Value";
				worksheet.Cell("B3").Value = reportData.TotalSoldItemsValue;

				using (var stream = new MemoryStream())
				{
					workbook.SaveAs(stream);
					byte[] bytes = stream.ToArray();
					return Task.FromResult(bytes);
				}
			}
		}
	}
}
