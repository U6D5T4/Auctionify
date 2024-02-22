using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Models.Report;
using Auctionify.Core.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Auctionify.Infrastructure.Services.ReportGeneratingServices
{
	public class PdfReportGeneratorService : IPdfReportGeneratorService
	{
		public Task<byte[]> GenerateReportAsync(ReportData reportData, User user)
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
                        column.Item().Text($"User: {user.FirstName} {user.LastName}", TextStyle.Default.Bold().Size(16));
                        column.Item().Text($"Total Sold Items: {reportData.TotalSoldItems}", TextStyle.Default.Size(14));
                        column.Item().Text($"Total Cost Of Sold Items: {reportData.TotalCostOfSoldItems.ToString()}", TextStyle.Default.Size(14));

                        column.Item().Padding(5);

                        column.Item().Text("Monthly Breakdown:", TextStyle.Default.Bold().Size(16));

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
                                header.Cell().Text("Month", TextStyle.Default.Bold());
                                header.Cell().Text("Sold Lots Count", TextStyle.Default.Bold());
                                header.Cell().Text("Total Sold Amount", TextStyle.Default.Bold());
                                header.Cell().Text("Created Lots Count", TextStyle.Default.Bold());
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
                return Task.FromResult(stream.ToArray());
            }
		}
	}
}
