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
		private readonly Mock<IXlsxReportGeneratorService> _xlsxReportGeneratorServiceMock;
		private readonly Mock<IPdfReportGeneratorService> _pdfReportGeneratorServiceMock;
		private readonly UserManager<User> _userManager;
		private readonly ICurrentUserService _currentUserService;

		public GenerateReportQueryHandlerTest()
		{
			_reportDataRepositoryMock = new Mock<IReportDataRepository>();
			_xlsxReportGeneratorServiceMock = new Mock<IXlsxReportGeneratorService>();
			_pdfReportGeneratorServiceMock = new Mock<IPdfReportGeneratorService>();

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
			var reportData = new ReportData
			{
				TotalItemsCount = 10,
				SoldItemsCount = 5,
				TotalSoldItemsValue = 1000
			};

			var byteArray = new byte[] { 0x01, 0x02, 0x03 };

			var reportDataMock = new Mock<IReportDataRepository>();
			var pdfReportServiceMock = new Mock<IPdfReportGeneratorService>();

			reportDataMock
				.Setup(x => x.GetReportDataAsync(It.IsAny<ReportRequest>()))
				.ReturnsAsync(reportData);

			pdfReportServiceMock
				.Setup(x => x.GenerateReportAsync(It.IsAny<ReportData>(), It.IsAny<User>()))
				.ReturnsAsync(byteArray);

			var handler = new GenerateReportHandler(
				reportDataMock.Object,
				_currentUserService,
				_userManager,
				_xlsxReportGeneratorServiceMock.Object,
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
			var reportData = new ReportData
			{
				TotalItemsCount = 10,
				SoldItemsCount = 5,
				TotalSoldItemsValue = 1000
			};

			var byteArray = new byte[] { 0x01, 0x02, 0x03 };

			var reportDataMock = new Mock<IReportDataRepository>();
			var xlsxReportServiceMock = new Mock<IXlsxReportGeneratorService>();

			reportDataMock
				.Setup(x => x.GetReportDataAsync(It.IsAny<ReportRequest>()))
				.ReturnsAsync(reportData);

			xlsxReportServiceMock
				.Setup(x => x.GenerateReportAsync(It.IsAny<ReportData>(), It.IsAny<User>()))
				.ReturnsAsync(byteArray);

			var handler = new GenerateReportHandler(
				reportDataMock.Object,
				_currentUserService,
				_userManager,
				xlsxReportServiceMock.Object,
				_pdfReportGeneratorServiceMock.Object
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
