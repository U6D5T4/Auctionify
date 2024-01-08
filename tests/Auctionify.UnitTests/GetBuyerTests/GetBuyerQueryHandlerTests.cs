using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Common.Options;
using Auctionify.Application.Features.Users.Queries.GetBuyer;
using Auctionify.Core.Entities;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;

namespace Auctionify.UnitTests.GetBuyerTests
{
	public class GetBuyerQueryHandlerTests : IDisposable
	{
		#region Initialization

		private readonly IMapper _mapper;
		private readonly Mock<ICurrentUserService> _currentUserServiceMock;
		private readonly UserManager<User> _userManager;
		private readonly IRateRepository _rateRepository;

		public GetBuyerQueryHandlerTests()
		{
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
		}

		#endregion

		#region Tests

		[Fact]
		public async Task GetBuyerQueryHandler_WhenCalled_ReturnsBuyerResponse()
		{
			// Arrange
			var query = new GetBuyerQuery();
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

			var handler = new GetBuyerQueryHandler(
				_currentUserServiceMock.Object,
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
