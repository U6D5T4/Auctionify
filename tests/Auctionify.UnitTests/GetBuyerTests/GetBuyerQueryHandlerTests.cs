using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Common.Models.Requests;
using Auctionify.Application.Common.Options;
using Auctionify.Application.Features.Users.Queries.GetBuyer;
using Auctionify.Core.Entities;
using Auctionify.Infrastructure.Persistence;
using Auctionify.Infrastructure.Repositories;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;

namespace Auctionify.UnitTests.GetBuyerTests
{
	public class GetBuyerQueryHandlerTests
	{
		#region Initialization

		private readonly IMapper _mapper;
		private readonly UserManager<User> _userManager;
		private readonly ICurrentUserService _currentUserService;
		private readonly IRateRepository _rateRepository;

		public GetBuyerQueryHandlerTests()
		{
			var mockDbContext = DbContextMock.GetMock<Rate, ApplicationDbContext>(EntitiesSeeding.GetRates(), ctx => ctx.Rates);
			mockDbContext = DbContextMock.GetMock(EntitiesSeeding.GetLotStatuses(), ctx => ctx.LotStatuses, mockDbContext);

			var configuration = new MapperConfiguration(
				cfg =>
					cfg.AddProfiles(
						new List<Profile>
						{
							new Application.Common.Profiles.MappingProfiles(),
							new Application.Features.Users.Profiles.MappingProfiles(),
						}
					)
			);
			_mapper = new Mapper(configuration);

			_userManager = EntitiesSeeding.GetUserManagerMock();
			_currentUserService = EntitiesSeeding.GetCurrentUserServiceMock();

			_rateRepository = new RateRepository(mockDbContext.Object);
		}

		#endregion

		#region Tests

		[Fact]
		public async Task GetBuyerQueryHandler_WhenCalled_ReturnsBuyerResponse()
		{
			// Arrange
			var query = new GetBuyerQuery
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

			var handler = new GetBuyerQueryHandler(
				_currentUserService,
				_userManager,
				blobServiceMock.Object,
				azureBlobStorageOptionsMock.Object,
				_mapper,
				_rateRepository
			);

			// Act
			var result = await handler.Handle(query, default);

			// Assert
			result.Should().NotBeNull();
			result.Should().BeOfType<GetBuyerResponse>();
			result.ProfilePictureUrl.Should().BeEquivalentTo(testUrl);
		}

		#endregion
	}
}
