using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Common.Models.Report;
using Auctionify.Application.Features.Reports.Query.Generate;
using Auctionify.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Auctionify.UnitTests.GenerateReportTests
{
	public class GenerateReportQueryHandlerTest
	{
		#region Initialization

		private readonly Mock<IReportDataRepository> _reportDataRepositoryMock;
		private readonly UserManager<User> _userManager;
		private readonly ICurrentUserService _currentUserService;

		public GenerateReportQueryHandlerTest()
		{
			_reportDataRepositoryMock = new Mock<IReportDataRepository>();

			_userManager = EntitiesSeeding.GetUserManagerMock();
			_currentUserService = EntitiesSeeding.GetCurrentUserServiceMock();
		}

		#endregion

		[Fact]
		public async Task Handle_GivenValidPdfRequest_ReturnsPdfByteArray()
		{
			// Arrange
			var user = new User
			{
				Id = 1,
				Email = "test@example.com",
				IsDeleted = false
			};

			var monthlyReport = new MonthlyReportData
			{
				ReportMonth = "09",
				CreatedLotsCount = 1,
				SoldLotsCount = 1,
				SoldLotsTotalCost = 1,
			};

			var reportData = new ReportData
			{
				TotalSoldItems = 10,
				TotalCostOfSoldItems = 1000,
			};

			reportData.MonthlyReports.Add(monthlyReport);

			var byteArray = new byte[] { 0x01, 0x02, 0x03 };

			var reportDataMock = new Mock<IReportDataRepository>();
			var pdfReportServiceMock = new Mock<IReportService>();

			reportDataMock
				.Setup(x => x.GetReportDataAsync(It.IsAny<ReportRequest>()))
				.ReturnsAsync(reportData);

			pdfReportServiceMock
				.Setup(x => x.GenerateReportAsync(It.IsAny<ReportData>(), It.IsAny<User>(), ReportType.PDF))
				.Returns(byteArray);

			var handler = new GenerateReportHandler(
				reportDataMock.Object,
				_currentUserService,
				_userManager,
				pdfReportServiceMock.Object
			);

			var query = new GenerateReportQuery { Format = ReportType.PDF,  MonthsDuration = 1 };

			// Act
			var result = await handler.Handle(query, CancellationToken.None);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(byteArray, result);
		}

		[Fact]
		public async Task Handle_GivenValidXlsxRequest_ReturnsPdfByteArray()
		{
			// Arrange
			var user = new User
			{
				Id = 1,
				Email = "test@example.com",
				IsDeleted = false
			};

			var monthlyReport = new MonthlyReportData
			{
				ReportMonth = "09",
				CreatedLotsCount = 1,
				SoldLotsCount = 1,
				SoldLotsTotalCost = 1,
			};

			var reportData = new ReportData
			{
				TotalSoldItems = 10,
				TotalCostOfSoldItems = 1000,
			};

			reportData.MonthlyReports.Add(monthlyReport);

			var byteArray = new byte[] { 0x01, 0x02, 0x03 };

			var reportDataMock = new Mock<IReportDataRepository>();
			var xlsxReportServiceMock = new Mock<IReportService>();

			reportDataMock
				.Setup(x => x.GetReportDataAsync(It.IsAny<ReportRequest>()))
				.ReturnsAsync(reportData);

			xlsxReportServiceMock
				.Setup(x => x.GenerateReportAsync(It.IsAny<ReportData>(), It.IsAny<User>(), ReportType.XLSX))
				.Returns(byteArray);

			var handler = new GenerateReportHandler(
				reportDataMock.Object,
				_currentUserService,
				_userManager,
				xlsxReportServiceMock.Object
			);

			var query = new GenerateReportQuery { Format = ReportType.XLSX, MonthsDuration = 1 };

			// Act
			var result = await handler.Handle(query, CancellationToken.None);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(byteArray, result);
		}
	}
}
