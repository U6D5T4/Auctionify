using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Common.Models.Report;
using Auctionify.Application.Features.Reports.Query.Generate;
using Auctionify.Core.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moq;
using System.Reflection.Metadata;

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
			var user = new User { Email = "test@example.com", IsDeleted = false };
			var reportData = new ReportData { TotalItemsCount = 10, SoldItemsCount = 5, TotalSoldItemsValue = 1000 };
			var byteArray = new byte[] { 0x01, 0x02, 0x03 };


			_reportDataRepositoryMock.Setup(x => x.GetReportDataAsync(It.IsAny<ReportRequest>())).ReturnsAsync(reportData);
			_pdfReportGeneratorServiceMock.Verify(x => x.GenerateReportAsync(It.IsAny<ReportData>(), It.IsAny<User>()), Times.Once);

			var handler = new GenerateReportHandler(
				_reportDataRepositoryMock.Object,
				_currentUserService,
				_userManager,
				_xlsxReportGeneratorServiceMock.Object,
				_pdfReportGeneratorServiceMock.Object
			);

			var query = new GenerateReportQuery { Format = "PDF", MonthsDuration = 1 };

			// Act
			var result = await handler.Handle(query, CancellationToken.None);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(byteArray, result);
			_pdfReportGeneratorServiceMock.Verify(x => x.GenerateReportAsync(reportData, user), Times.Once);
		}

		[Fact]
		public async Task Handle_XlsxRequestForNonExistingUser_ReturnsEmptyByteArray()
		{
			// Arrange:
			var query = new GenerateReportQuery { Format = "XLSX", MonthsDuration = 1 };

			var handler = new GenerateReportHandler(
				_reportDataRepositoryMock.Object,
				_currentUserService,
				_userManager,
				_xlsxReportGeneratorServiceMock.Object,
				_pdfReportGeneratorServiceMock.Object
			);

			// Act
			var result = await handler.Handle(query, CancellationToken.None);

			// Assert
			Assert.NotNull(result);
			Assert.Empty(result);
		}

	}
}
