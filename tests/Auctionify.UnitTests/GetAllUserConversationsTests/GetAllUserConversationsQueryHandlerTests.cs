using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Common.Options;
using Auctionify.Application.Features.Chats.Queries.GetAllUserConversations;
using Auctionify.Core.Entities;
using Auctionify.Core.Enums;
using Auctionify.Infrastructure.Persistence;
using Auctionify.Infrastructure.Repositories;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MockQueryable.Moq;
using Moq;

namespace Auctionify.UnitTests.GetAllUserConversationsTests
{
	public class GetAllUserConversationsQueryHandlerTests : IDisposable
	{
		#region Initialization

		private readonly IConversationRepository _conversationRepository;
		private readonly IMapper _mapper;
		private readonly Mock<ICurrentUserService> _currentUserServiceMock;
		private readonly Mock<UserManager<User>> _userManagerMock;
		private readonly Mock<IBlobService> _blobServiceMock;
		private readonly Mock<IOptions<AzureBlobStorageOptions>> _blobStorageOptionsMock;

		public GetAllUserConversationsQueryHandlerTests()
		{
			var mockDbContext = DbContextMock.GetMock<Conversation, ApplicationDbContext>(
				EntitiesSeeding.GetConversations(),
				ctx => ctx.Conversations
			);

			mockDbContext = DbContextMock.GetMock(
				EntitiesSeeding.GetChatMessages(),
				ctx => ctx.ChatMessages,
				mockDbContext
			);

			var configuration = new MapperConfiguration(
				cfg =>
					cfg.AddProfiles(
						new List<Profile>
						{
							new Application.Common.Profiles.MappingProfiles(),
							new Application.Features.Lots.Profiles.MappingProfiles(),
							new Application.Features.Users.Profiles.MappingProfiles(),
							new Application.Features.Chats.Profiles.MappingProfiles(),
						}
					)
			);

			_mapper = new Mapper(configuration);

			_currentUserServiceMock = new Mock<ICurrentUserService>();
			_userManagerMock = new Mock<UserManager<User>>(
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
			_blobServiceMock = new Mock<IBlobService>();
			_blobStorageOptionsMock = new Mock<IOptions<AzureBlobStorageOptions>>();
			_conversationRepository = new ConversationRepository(mockDbContext.Object);
		}

		#endregion

		#region Tests

		[Fact]
		public async Task Handle_ShouldReturnAllUserConversations()
		{
			// Arrange
			var query = new GetAllUserConversationsQuery();

			var user = new User
			{
				Id = 1,
				Email = "buyer@example.com",
				ProfilePicture = "test-profile-picture.png"
			};

			var roles = new List<string> { UserRole.Buyer.ToString() };

			var mock = new List<User> { user }.AsQueryable().BuildMockDbSet();
			_userManagerMock.Setup(m => m.Users).Returns(mock.Object);
			_currentUserServiceMock.Setup(m => m.UserEmail).Returns(user.Email);

			_userManagerMock.Setup(m => m.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(roles);

			_blobStorageOptionsMock
				.SetupGet(m => m.Value)
				.Returns(
					new AzureBlobStorageOptions
					{
						UserProfilePhotosFolderName = "test-user-profile-photos"
					}
				);

			_blobServiceMock
				.Setup(m => m.GetBlobUrl(It.IsAny<string>(), It.IsAny<string>()))
				.Returns("https://test.blob.storage.com/test-user-profile-photos/test-profile-picture.png");

			var handler = new GetAllUserConversationsQueryHandler(
				_conversationRepository,
				_mapper,
				_currentUserServiceMock.Object,
				_userManagerMock.Object,
				_blobServiceMock.Object,
				_blobStorageOptionsMock.Object
			);

			// Act
			var result = await handler.Handle(query, CancellationToken.None);

			// Assert
			result.Should().NotBeNull();
			result.Should().BeOfType<GetAllUserConversationsResponse>();
			result.Conversations.Count.Should().Be(2);
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
				_blobServiceMock.Reset();
				_blobStorageOptionsMock.Reset();
				_userManagerMock.Reset();
				_currentUserServiceMock.Reset();
			}
		}

		#endregion
	}
}
