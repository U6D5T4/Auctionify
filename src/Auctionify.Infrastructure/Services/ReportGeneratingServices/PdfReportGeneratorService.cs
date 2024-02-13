using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Models.Report;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Auctionify.Infrastructure.Services.ReportGeneratingServices
{
	public class PdfReportGeneratorService : IPdfReportGeneratorService
	{
		public Task<byte[]> GenerateReportAsync(ReportData reportData)
		{
			var document = Document.Create(container =>
			{
				container.Page(page =>
				{
					page.Margin(50);
					page.Size(PageSizes.A4);
					page.Content().Column(column =>
					{
						column.Item().Text($"Total Items: {reportData.TotalItemsCount}", TextStyle.Default.Size(20));
						column.Item().Text($"Sold Items: {reportData.SoldItemsCount}", TextStyle.Default.Size(20));
						column.Item().Text($"Total Sold Value: {reportData.TotalSoldItemsValue}", TextStyle.Default.Size(20));
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
