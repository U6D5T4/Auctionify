using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Models.Report;
using Auctionify.Core.Entities;
using ClosedXML.Excel;

namespace Auctionify.Infrastructure.Services.ReportGeneratingServices
{
	public class XlsxReportGeneratorService : IXlsxReportGeneratorService
	{
		public Task<byte[]> GenerateReportAsync(ReportData reportData, User user)
		{
			using (var workbook = new XLWorkbook())
			{
				var worksheet = workbook.Worksheets.Add("User Report");

				worksheet.Cell("A1").Value = $"Report for {user.FirstName} {user.LastName}";
				worksheet.Cell("A1").Style.Font.Bold = true;
				worksheet.Range("A1:B1").Merge().Style.Font.SetFontSize(16);

				worksheet.Cell("A4").Value = "Total Items";
				worksheet.Cell("B4").Value = reportData.TotalItemsCount;
				worksheet.Cell("A5").Value = "Sold Items";
				worksheet.Cell("B5").Value = reportData.SoldItemsCount;
				worksheet.Cell("A6").Value = "Total Sold Value";
				worksheet.Cell("B6").Value = reportData.TotalSoldItemsValue.ToString();

				worksheet.Range("A4:A6").Style.Font.Bold = true;
				worksheet.Columns().AdjustToContents();

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
