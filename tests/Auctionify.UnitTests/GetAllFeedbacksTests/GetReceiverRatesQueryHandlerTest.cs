using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Common.Interfaces;
using Auctionify.Core.Entities;
using Auctionify.Infrastructure.Persistence;
using Auctionify.Infrastructure.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moq;
using Auctionify.Application.Common.DTOs;
using Auctionify.Application.Common.Models.Requests;
using Auctionify.Application.Features.Rates.Queries.GetReceiverRates;
using FluentAssertions;
using Auctionify.Application.Common.Options;
using Microsoft.Extensions.Options;

namespace Auctionify.UnitTests.GetAllFeedbacksTests
{
    public class GetReceiverRatesQueryHandlerTest
	{
		private readonly IMapper _mapper;
		private readonly IRateRepository _rateRepository;
		private readonly Mock<ICurrentUserService> _currentUserServiceMock;
		private readonly UserManager<User> _userManager;

		public GetReceiverRatesQueryHandlerTest()
		{
			var mockDbContext = DbContextMock.GetMock<Rate, ApplicationDbContext>(
				EntitiesSeeding.GetRates(),
				ctx => ctx.Rates
			);

			var configuration = new MapperConfiguration(
				cfg =>
					cfg.AddProfiles(
						new List<Profile>
						{
							new Application.Common.Profiles.MappingProfiles(),
							new Application.Features.Rates.Profiles.MappingProfiles(),
						}
					)
			);

			_rateRepository = new RateRepository(mockDbContext.Object);
			_currentUserServiceMock = new Mock<ICurrentUserService>();
			_userManager = EntitiesSeeding.GetUserManagerMock();
			_mapper = new Mapper(configuration);
		}

		[Fact]
		public async Task GetAllReceiverRatesQueryHandler_WhenCalled_ReturnsAllRates()
		{
			var allRates = EntitiesSeeding.GetRates();
			var query = new GetAllReceiverRatesQuery
			{
				PageRequest = new PageRequest { PageIndex = 0, PageSize = 10 }
			};
			var testUrl = "test-url";
			var blobServiceMock = new Mock<IBlobService>();
			var azureBlobStorageOptionsMock = new Mock<IOptions<AzureBlobStorageOptions>>();

			var user = new User { Id = 1, Email = "user@example.com" };

			azureBlobStorageOptionsMock
				.Setup(x => x.Value)
				.Returns(
					new AzureBlobStorageOptions
					{
						ContainerName = "auctionify-files",
						PhotosFolderName = "photos",
						AdditionalDocumentsFolderName = "additional-documents",
						UserProfilePhotosFolderName = "user-profile-photos"
					}
				);

			blobServiceMock
				.Setup(x => x.GetBlobUrl(It.IsAny<string>(), It.IsAny<string>()))
				.Returns(testUrl);

			var handler = new GetAllReceiverRatesQueryHandler(
				_currentUserServiceMock.Object,
				_userManager,
				_mapper,
				_rateRepository,
				azureBlobStorageOptionsMock.Object,
				blobServiceMock.Object
			);

			var result = await handler.Handle(query, default);

			result.Should().BeOfType<GetListResponseDto<GetAllReceiverRatesResponse>>();
			var countLots = allRates.Where(l => l.SenderId == user.Id).Count();

			result.Count.Should().Be(countLots);
		}

		[Fact]
		public async Task GetAllReceiverRatesQueryHandler_WhenCalled_ReturnsEmptyList()
		{
			var allRates = new List<Rate>();
			var mockDbContext = DbContextMock.GetMock<Rate, ApplicationDbContext>(
				allRates,
				ctx => ctx.Rates
			);
			var rateRepository = new RateRepository(mockDbContext.Object);
			var query = new GetAllReceiverRatesQuery
			{
				PageRequest = new PageRequest { PageIndex = 0, PageSize = 10 }
			};

			var testUrl = "test-url";
			var blobServiceMock = new Mock<IBlobService>();
			var azureBlobStorageOptionsMock = new Mock<IOptions<AzureBlobStorageOptions>>();

			azureBlobStorageOptionsMock
				.Setup(x => x.Value)
				.Returns(
					new AzureBlobStorageOptions
					{
						ContainerName = "auctionify-files",
						PhotosFolderName = "photos",
						AdditionalDocumentsFolderName = "additional-documents",
						UserProfilePhotosFolderName = "user-profile-photos"
					}
				);

			blobServiceMock
				.Setup(x => x.GetBlobUrl(It.IsAny<string>(), It.IsAny<string>()))
				.Returns(testUrl);

			var handler = new GetAllReceiverRatesQueryHandler(
				_currentUserServiceMock.Object,
				_userManager,
				_mapper,
				rateRepository,
				azureBlobStorageOptionsMock.Object,
				blobServiceMock.Object
			);

			var result = await handler.Handle(query, default);

			result.Should().BeOfType<GetListResponseDto<GetAllReceiverRatesResponse>>();
			result.Count.Should().Be(allRates.Count);
		}
	}
}
