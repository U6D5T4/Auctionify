using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
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
	public class GetSellerQueryHandlerTests : IDisposable
	{
		#region Initialization

		private readonly IMapper _mapper;
		private readonly Mock<ICurrentUserService> _currentUserServiceMock;
		private readonly UserManager<User> _userManager;
		private readonly ILotRepository _lotRepository;
		private readonly ILotStatusRepository _lotStatusRepository;

		public GetSellerQueryHandlerTests()
		{
			var mockDbContext = DbContextMock.GetMock<Lot, ApplicationDbContext>(EntitiesSeeding.GetLots(), ctx => ctx.Lots);
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

			_currentUserServiceMock = new Mock<ICurrentUserService>();
			_userManager = EntitiesSeeding.GetUserManagerMock();

			_lotRepository = new LotRepository(mockDbContext.Object);
			_lotStatusRepository = new LotStatusRepository(mockDbContext.Object);
		}

		#endregion

		#region Tests

		[Fact]
		public async Task GetSellerQueryHandler_WhenCalled_ReturnsSellerResponse()
		{
			// Arrange
			var query = new GetSellerQuery();
			var testUrl = "test-url";
			var blobServiceMock = new Mock<IBlobService>();
			var azureBlobStorageOptionsMock = new Mock<IOptions<AzureBlobStorageOptions>>();

			_currentUserServiceMock.Setup(x => x.UserEmail).Returns(It.IsAny<string>());

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
				_currentUserServiceMock.Object,
				_userManager,
				blobServiceMock.Object,
				azureBlobStorageOptionsMock.Object,
				_mapper,
				_lotRepository,
				_lotStatusRepository
			);

			// Act
			var result = await handler.Handle(query, CancellationToken.None);

			// Assert
			result.Should().BeOfType<GetSellerResponse>();
			result.ProfilePictureUrl.Should().Be(testUrl);
		}

		#endregion

		#region Deinitialization

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				_currentUserServiceMock.Reset();
			}
		}

		#endregion
	}
}
