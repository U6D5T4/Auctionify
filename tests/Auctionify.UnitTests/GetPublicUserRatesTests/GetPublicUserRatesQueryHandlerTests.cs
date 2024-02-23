using Auctionify.Application.Common.DTOs;
using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Common.Options;
using Auctionify.Application.Features.Rates.Queries.GetPublicUserRates;
using Auctionify.Core.Entities;
using Auctionify.Infrastructure.Persistence;
using Auctionify.Infrastructure.Repositories;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using User = Auctionify.Core.Entities.User;

namespace Auctionify.UnitTests.GetPublicUserRatesTests
{
	public class GetPublicUserRatesQueryHandlerTests
	{
		#region Initialization

		private readonly IMapper _mapper;
		private readonly IRateRepository _rateRepository;

		public GetPublicUserRatesQueryHandlerTests()
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
							new Application.Features.Users.Profiles.MappingProfiles(),
							new Application.Features.Rates.Profiles.MappingProfiles()
						}
					)
			);
			_mapper = new Mapper(configuration);

			_rateRepository = new RateRepository(mockDbContext.Object);
		}

		#endregion

		#region Tests

		[Fact]
		public async Task Handle_ValidUserId_ReturnsRates()
		{
			var blobServiceMock = new Mock<IBlobService>();
			var azureBlobStorageOptionsMock = new Mock<IOptions<AzureBlobStorageOptions>>();

			var userManagerMock = new Mock<UserManager<User>>(
				Mock.Of<IUserStore<User>>(),
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
			);

			var user = new User
			{
				Id = 1,
				UserName = "TestUserName",
				Email = "test@test.COM",
				EmailConfirmed = true,
				PhoneNumber = "123456789",
				FirstName = "TestFirstName",
				LastName = "TestLastName",
				AboutMe = "TestAboutMe",
				ProfilePicture = "TestProfilePicture.png",
				IsDeleted = false,
			};

			userManagerMock.Setup(m => m.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);

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
				.Returns("test-url");

			// Arrange
			var handler = new GetPublicUserRatesQueryHandler(
				userManagerMock.Object,
				_mapper,
				blobServiceMock.Object,
				azureBlobStorageOptionsMock.Object,
				_rateRepository
			);

			var query = new GetPublicUserRatesQuery { UserId = "1" };

			// Act
			var result = await handler.Handle(query, CancellationToken.None);

			// Assert
			result.Should().NotBeNull();
			result.Should().BeOfType<GetListResponseDto<GetPublicUserRatesResponse>>();
			result.Items.Should().NotBeNullOrEmpty();
			result.Items.Count.Should().Be(1);
		}

		[Fact]
		public async Task Handle_InvalidUserId_ReturnsEmptyList()
		{
			var blobServiceMock = new Mock<IBlobService>();
			var azureBlobStorageOptionsMock = new Mock<IOptions<AzureBlobStorageOptions>>();

			var userManagerMock = new Mock<UserManager<User>>(
				Mock.Of<IUserStore<User>>(),
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
			);

			userManagerMock
				.Setup(m => m.FindByIdAsync(It.IsAny<string>()))
				.ReturnsAsync((User)null);

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
				.Returns("test-url");

			// Arrange
			var handler = new GetPublicUserRatesQueryHandler(
				userManagerMock.Object,
				_mapper,
				blobServiceMock.Object,
				azureBlobStorageOptionsMock.Object,
				_rateRepository
			);

			var query = new GetPublicUserRatesQuery { UserId = "1" };

			// Act
			var result = await handler.Handle(query, CancellationToken.None);

			// Assert
			result.Should().BeOfType<GetListResponseDto<GetPublicUserRatesResponse>>();
			result.Items.Count.Should().Be(0);
		}

		#endregion
	}
}
