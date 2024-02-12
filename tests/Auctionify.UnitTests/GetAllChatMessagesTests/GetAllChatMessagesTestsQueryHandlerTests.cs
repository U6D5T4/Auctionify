using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Features.Chats.Queries.GetAllChatMessages;
using Auctionify.Core.Entities;
using Auctionify.Infrastructure.Persistence;
using Auctionify.Infrastructure.Repositories;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using MockQueryable.Moq;
using Moq;

namespace Auctionify.UnitTests.GetAllChatMessagesTests
{
	public class GetAllChatMessagesTestsQueryHandlerTests : IDisposable
	{
		#region Initialization

		private readonly IConversationRepository _conversationRepository;
		private readonly IChatMessageRepository _chatMessageRepository;
		private readonly IMapper _mapper;
		private readonly Mock<ICurrentUserService> _currentUserServiceMock;
		private readonly Mock<UserManager<User>> _userManagerMock;

		public GetAllChatMessagesTestsQueryHandlerTests()
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

			_conversationRepository = new ConversationRepository(mockDbContext.Object);
			_chatMessageRepository = new ChatMessageRepository(mockDbContext.Object);
		}

		#endregion

		#region Tests

		[Fact]
		public async Task GetAllChatMessagesQueryHandler_ShouldReturnAllChatMessages()
		{
			// Arrange
			var query = new GetAllChatMessagesQuery { ConversationId = 1 };

			var user = new User
			{
				Id = 1,
				Email = "buyer@example.com",
				ProfilePicture = "test-profile-picture.png"
			};

			var mock = new List<User> { user }.AsQueryable().BuildMockDbSet();
			_userManagerMock.Setup(m => m.Users).Returns(mock.Object);
			_currentUserServiceMock.Setup(m => m.UserEmail).Returns(user.Email);

			var handler = new GetAllChatMessagesQueryHandler(
				_conversationRepository,
				_chatMessageRepository,
				_mapper,
				_currentUserServiceMock.Object,
				_userManagerMock.Object
			);

			// Act
			var result = await handler.Handle(query, CancellationToken.None);

			// Assert
			result.Should().NotBeNull();
			result.Should().BeOfType<GetAllChatMessagesResponse>();
			result.ChatMessages.Count.Should().Be(3);
		}

		[Fact]
		public async Task GetAllChatMessagesQueryHandler_ShouldThrowValidationException()
		{
			// Arrange
			var query = new GetAllChatMessagesQuery { ConversationId = 3 };

			var user = new User
			{
				Id = 1,
				Email = "buyer@example.com",
				ProfilePicture = "test-profile-picture.png"
			};

			var mock = new List<User> { user }.AsQueryable().BuildMockDbSet();
			_userManagerMock.Setup(m => m.Users).Returns(mock.Object);
			_currentUserServiceMock.Setup(m => m.UserEmail).Returns(user.Email);

			var handler = new GetAllChatMessagesQueryHandler(
				_conversationRepository,
				_chatMessageRepository,
				_mapper,
				_currentUserServiceMock.Object,
				_userManagerMock.Object
			);

			// Act (throws ValidationException)
			Func<Task> act = async () => await handler.Handle(query, CancellationToken.None);

			// Assert
			await act.Should()
				.ThrowAsync<ValidationException>()
				.WithMessage(
					"Validation failed: \r\n -- ConversationId: You are not allowed to access this conversation Severity: Error"
				);
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
				_userManagerMock.Reset();
				_currentUserServiceMock.Reset();
			}
		}

		#endregion
	}
}
