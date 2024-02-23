using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Models.Report;
using Auctionify.Core.Entities;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF.Helpers;
using ClosedXML.Excel;

namespace Auctionify.Infrastructure.Services
{
	public class ReportGeneratorService : IReportService
	{
		public byte[] GenerateReport(ReportData reportData, User user, ReportType format)
		{
			switch (format)
			{
				case ReportType.PDF:
					return GeneratePdfReport(reportData, user);
				case ReportType.XLSX:
					return GenerateXlsxReport(reportData, user);
				default:
					return Array.Empty<byte>();
			}
		}

		private byte[] GeneratePdfReport(ReportData reportData, User user)
		{
			var document = Document.Create(container =>
			{
				container.Page(page =>
				{
					page.Margin(50);
					page.Size(PageSizes.A4);
					page.Header().Text($"Auctionify Sales Report").Bold().FontSize(20);

					page.Content().Column(column =>
					{
						column.Item().Text($"User: {user.FirstName} {user.LastName}").Bold().FontSize(16);
						column.Item().Text($"Total Sold Items: {reportData.TotalSoldItems}").Bold().FontSize(14);
						column.Item().Text($"Total Cost Of Sold Items: {reportData.TotalCostOfSoldItems.ToString()}").FontSize(14);

						column.Item().Padding(5);

						column.Item().Text("Monthly Breakdown:").Bold().FontSize(16);

						column.Item().Table(table =>
						{
							table.ColumnsDefinition(columns =>
							{
								columns.RelativeColumn(3);
								columns.RelativeColumn(3);
								columns.RelativeColumn(3);
								columns.RelativeColumn(3);
							});

							table.Header(header =>
							{
								header.Cell().Text("Month").Bold();
								header.Cell().Text("Sold Lots Count").Bold();
								header.Cell().Text("Total Sold Amount").Bold();
								header.Cell().Text("Created Lots Count").Bold();
							});

							foreach (var monthlyData in reportData.MonthlyReports)
							{
								table.Cell().Text(monthlyData.ReportMonth);
								table.Cell().Text(monthlyData.SoldLotsCount.ToString());
								table.Cell().Text(monthlyData.SoldLotsTotalCost.ToString());
								table.Cell().Text(monthlyData.CreatedLotsCount.ToString());
							}
						});
					});
				});
			});

			using (var stream = new MemoryStream())
			{
				document.GeneratePdf(stream);
				return stream.ToArray();
			}
		}

		private byte[] GenerateXlsxReport(ReportData reportData, User user)
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
					return bytes;
				}
			}
		}
	}
}
