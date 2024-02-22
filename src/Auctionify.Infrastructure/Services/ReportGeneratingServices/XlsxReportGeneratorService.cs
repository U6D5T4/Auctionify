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
				worksheet.Range("A1:D1").Merge().Style.Font.SetFontSize(16);

				worksheet.Cell("A3").Value = "Total Sold Items";
				worksheet.Cell("B3").Value = reportData.TotalSoldItems;
				worksheet.Cell("A4").Value = "Total Cost Of Sold Items";
				worksheet.Cell("B4").Value = reportData.TotalCostOfSoldItems.ToString();

				worksheet.Range("A3:A4").Style.Font.Bold = true;

				int currentRow = 6;
				worksheet.Cell($"A{currentRow}").Value = "Monthly Breakdown";
				worksheet.Range($"A{currentRow}:D{currentRow}").Merge().Style.Font.Bold = true;
				currentRow++;

				worksheet.Cell($"A{currentRow}").Value = "Month-Year";
				worksheet.Cell($"B{currentRow}").Value = "Sold Lots Count";
				worksheet.Cell($"C{currentRow}").Value = "Total Sold Amount";
				worksheet.Cell($"D{currentRow}").Value = "Created Lots Count";
				worksheet.Range($"A{currentRow}:D{currentRow}").Style.Font.Bold = true;
				currentRow++;

				foreach (var monthlyData in reportData.MonthlyReports)
				{
					worksheet.Cell($"A{currentRow}").Value = monthlyData.ReportMonth;
					worksheet.Cell($"B{currentRow}").Value = monthlyData.SoldLotsCount;
					worksheet.Cell($"C{currentRow}").Value = monthlyData.SoldLotsTotalCost;
					worksheet.Cell($"D{currentRow}").Value = monthlyData.CreatedLotsCount;
					currentRow++;
				}

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
