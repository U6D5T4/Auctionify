using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Options;
using Auctionify.Application.Features.Users.Commands.Update;
using Auctionify.Core.Entities;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;

namespace Auctionify.UnitTests.UpdateUserTests
{
	public class UpdateUserCommandHandlerTests : IDisposable
	{
		#region Initialization

		private readonly IMapper _mapper;
		private readonly Mock<ICurrentUserService> _currentUserServiceMock;
		private readonly UserManager<User> _userManager;

		public UpdateUserCommandHandlerTests()
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
		public async Task UpdateUserCommandHandler_WhenNewProfilePictureIsProvided_ShouldUploadNewProfilePicture()
		{
			// Arrange
			_currentUserServiceMock.Setup(c => c.UserEmail).Returns("test@test.com");
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

			blobServiceMock.Setup(
				x => x.DeleteFileBlobAsync(It.IsAny<string>(), It.IsAny<string>())
			);

			blobServiceMock.Setup(
				x =>
					x.UploadFileBlobAsync(
						It.IsAny<IFormFile>(),
						It.IsAny<string>(),
						It.IsAny<string>()
					)
			);

			blobServiceMock
				.Setup(x => x.GetBlobUrl(It.IsAny<string>(), It.IsAny<string>()))
				.Returns(testUrl);

			var handler = new UpdateUserCommandHandler(
				_currentUserServiceMock.Object,
				_userManager,
				blobServiceMock.Object,
				azureBlobStorageOptionsMock.Object,
				_mapper
			);

			var updateCommand = new UpdateUserCommand
			{
				FirstName = "John",
				LastName = "Doe",
				PhoneNumber = "123456789",
				AboutMe = "Test user",
				ProfilePicture = new FormFile(
					new MemoryStream(),
					0,
					0,
					"profile_picture.jpg",
					"profile_picture.jpg"
				),
				DeleteProfilePicture = false
			};

			// Act
			var result = await handler.Handle(updateCommand, CancellationToken.None);

			// Assert
			blobServiceMock.Verify(
				b =>
					b.UploadFileBlobAsync(
						It.IsAny<IFormFile>(),
						It.IsAny<string>(),
						It.IsAny<string>()
					),
				Times.Once
			);
			result.Should().NotBeNull();
			result.Should().BeOfType<UpdatedUserResponse>();
			result.ProfilePictureUrl.Should().BeEquivalentTo(testUrl);
		}

		[Fact]
		public async Task UpdateUserCommandHandler_WhenNoNewProfilePictureIsProvidedAndDeleteProfilePictureIsTrue_ShouldDeleteProfilePicture()
		{
			// Arrange
			_currentUserServiceMock.Setup(c => c.UserEmail).Returns("test@test.com");
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

			blobServiceMock.Setup(
				x => x.DeleteFileBlobAsync(It.IsAny<string>(), It.IsAny<string>())
			);

			blobServiceMock.Setup(
				x =>
					x.UploadFileBlobAsync(
						It.IsAny<IFormFile>(),
						It.IsAny<string>(),
						It.IsAny<string>()
					)
			);

			blobServiceMock
				.Setup(x => x.GetBlobUrl(It.IsAny<string>(), It.IsAny<string>()))
				.Returns(testUrl);

			var handler = new UpdateUserCommandHandler(
				_currentUserServiceMock.Object,
				_userManager,
				blobServiceMock.Object,
				azureBlobStorageOptionsMock.Object,
				_mapper
			);

			var updateCommand = new UpdateUserCommand
			{
				FirstName = "John",
				LastName = "Doe",
				PhoneNumber = "123456789",
				AboutMe = "Test user",
				ProfilePicture = null,
				DeleteProfilePicture = true
			};

			// Act
			var result = await handler.Handle(updateCommand, CancellationToken.None);

			// Assert
			blobServiceMock.Verify(
				b =>
					b.UploadFileBlobAsync(
						It.IsAny<IFormFile>(),
						It.IsAny<string>(),
						It.IsAny<string>()
					),
				Times.Never
			);
			result.Should().NotBeNull();
			result.Should().BeOfType<UpdatedUserResponse>();
			result.ProfilePictureUrl.Should().BeNull();
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
