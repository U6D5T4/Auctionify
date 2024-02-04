using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Common.Models.Requests;
using Auctionify.Application.Common.Options;
using Auctionify.Application.Features.Users.Queries.GetSeller;
using Auctionify.Core.Entities;
using Auctionify.Infrastructure.Persistence;
using Auctionify.Infrastructure.Repositories;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;

namespace Auctionify.UnitTests.GetSellerTests
{
	public class GetSellerQueryHandlerTests
	{
		#region Initialization

		private readonly IMapper _mapper;
		private readonly UserManager<User> _userManager;
		private readonly ICurrentUserService _currentUserService;
		private readonly ILotRepository _lotRepository;
		private readonly ILotStatusRepository _lotStatusRepository;
		private readonly IRateRepository _rateRepository;

		public GetSellerQueryHandlerTests()
		{
			var mockDbContext = DbContextMock.GetMock<Lot, ApplicationDbContext>(
				EntitiesSeeding.GetLots(),
				ctx => ctx.Lots
			);
			mockDbContext = DbContextMock.GetMock(
				EntitiesSeeding.GetLotStatuses(),
				ctx => ctx.LotStatuses,
				mockDbContext
			);
			mockDbContext = DbContextMock.GetMock(
				EntitiesSeeding.GetRates(),
				ctx => ctx.Rates,
				mockDbContext
			);

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

			_lotRepository = new LotRepository(mockDbContext.Object);
			_lotStatusRepository = new LotStatusRepository(mockDbContext.Object);
			_rateRepository = new RateRepository(mockDbContext.Object);
		}

		#endregion

		#region Tests

		[Fact]
		public async Task GetSellerQueryHandler_WhenCalled_ReturnsSellerResponse()
		{
			// Arrange
			var query = new GetSellerQuery
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

			var handler = new GetSellerQueryHandler(
				_currentUserService,
				_userManager,
				blobServiceMock.Object,
				azureBlobStorageOptionsMock.Object,
				_mapper,
				_lotRepository,
				_lotStatusRepository,
				_rateRepository
			);

			// Act
			var result = await handler.Handle(query, CancellationToken.None);

			// Assert
			result.Should().BeOfType<GetSellerResponse>();
			result.ProfilePictureUrl.Should().BeEquivalentTo(testUrl);
		}

		#endregion
	}
}
