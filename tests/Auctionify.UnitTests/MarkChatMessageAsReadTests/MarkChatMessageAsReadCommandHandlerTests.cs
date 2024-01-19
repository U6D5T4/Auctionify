using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Features.Chats.Commands.MarkChatMessageAsRead;
using Auctionify.Core.Entities;
using Auctionify.Infrastructure.Persistence;
using Auctionify.Infrastructure.Repositories;
using FluentAssertions;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Auctionify.UnitTests.MarkChatMessageAsReadTests
{
	public class MarkChatMessageAsReadCommandHandlerTests : IDisposable
	{
		#region Initialization

		private readonly IChatMessageRepository _chatMessageRepository;
		private readonly IConversationRepository _conversationRepository;
		private readonly Mock<ICurrentUserService> _currentUserServiceMock;
		private readonly Mock<UserManager<User>> _userManagerMock;
		private readonly MarkChatMessageAsReadCommandValidator _validator;

		public MarkChatMessageAsReadCommandHandlerTests()
		{
			var mockDbContext = DbContextMock.GetMock<ChatMessage, ApplicationDbContext>(
				EntitiesSeeding.GetChatMessages(),
				ctx => ctx.ChatMessages
			);
			mockDbContext = DbContextMock.GetMock(
				EntitiesSeeding.GetConversations(),
				ctx => ctx.Conversations,
				mockDbContext
			);

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

			_currentUserServiceMock.Setup(x => x.UserEmail).Returns(It.IsAny<string>());

			var newUser = new User { Id = 1 };

			_userManagerMock
				.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
				.ReturnsAsync(newUser);

			_chatMessageRepository = new ChatMessageRepository(mockDbContext.Object);

			_conversationRepository = new ConversationRepository(mockDbContext.Object);

			_validator = new MarkChatMessageAsReadCommandValidator(
				_chatMessageRepository,
				_conversationRepository,
				_userManagerMock.Object,
				_currentUserServiceMock.Object
			);
		}

		#endregion

		#region Tests

		[Fact]
		public async Task Handle_GivenValidRequest_ShouldReturnMarkedChatMessageAsReadResponse()
		{
			// Arrange
			var sut = new MarkChatMessageAsReadCommandHandler(_chatMessageRepository);

			var request = new MarkChatMessageAsReadCommand { ChatMessageId = 1 };

			// Act
			var result = await sut.Handle(request, CancellationToken.None);

			// Assert
			result.Should().BeOfType<MarkedChatMessageAsReadResponse>();
			result.ChatMessageId.Should().Be(request.ChatMessageId);
			result.Success.Should().BeTrue();
			result.Message.Should().Be("Chat message marked as read.");
		}

		[Fact]
		public async Task Handle_GivenNonExistingChatMessageId_ShouldThrowValidationException()
		{
			// Arrange
			var request = new MarkChatMessageAsReadCommand { ChatMessageId = 100 };

			// Act
			var result = await _validator.ValidateAsync(request);

			// Assert
			result.Should().BeOfType<ValidationResult>();
			result.IsValid.Should().BeFalse();
			result.Errors.Should().NotBeEmpty();
			result.Errors.Count.Should().Be(1);
			result
				.Errors.First()
				.ErrorMessage.Should()
				.Be("Chat message with this Id does not exist");
		}

		[Fact]
		public async Task Handle_GivenChatMessageIdThatIsNotSentByCurrentUser_ShouldThrowValidationException()
		{
			// Arrange
			var request = new MarkChatMessageAsReadCommand { ChatMessageId = 1 };

			// Act
			var result = await _validator.ValidateAsync(request);

			// Assert
			result.Should().BeOfType<ValidationResult>();
			result.IsValid.Should().BeFalse();
			result.Errors.Should().NotBeEmpty();
			result.Errors.Count.Should().Be(1);
			result
				.Errors.First()
				.ErrorMessage.Should()
				.Be("Sender of the message cannot mark it as read");
		}

		[Fact]
		public async Task Handle_GivenChatMessageIdThatIsNotSentToCurrentUser_ShouldThrowValidationException()
		{
			// Arrange
			var newUser = new User { Id = 3 };

			_userManagerMock
				.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
				.ReturnsAsync(newUser);

			var request = new MarkChatMessageAsReadCommand { ChatMessageId = 2 };

			// Act
			var result = await _validator.ValidateAsync(request);

			// Assert
			result.Should().BeOfType<ValidationResult>();
			result.IsValid.Should().BeFalse();
			result.Errors.Should().NotBeEmpty();
			result.Errors.Count.Should().Be(1);
			result
				.Errors.First()
				.ErrorMessage.Should()
				.Be("You are not the receiver of this chat message");
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
