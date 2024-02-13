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
					page.Header().Text($"User Sales Report - {user.FirstName} {user.LastName}").Bold().FontSize(20);

					page.Content().Column(column =>
					{
						column.Item().Text($"User: {user.FirstName} {user.LastName}", TextStyle.Default.Bold().Size(16));

						column.Item().Text($"Total Items: {reportData.TotalItemsCount}", TextStyle.Default.Size(14));
						column.Item().Text($"Sold Items: {reportData.SoldItemsCount}", TextStyle.Default.Size(14));
						column.Item().Text($"Total Sold Value: {reportData.TotalSoldItemsValue}", TextStyle.Default.Size(14));
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
