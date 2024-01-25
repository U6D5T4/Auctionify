using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Features.Chats.Commands.CreateChatMessage;
using Auctionify.Application.Hubs;
using Auctionify.Core.Entities;
using Auctionify.Infrastructure.Persistence;
using Auctionify.Infrastructure.Repositories;
using AutoMapper;
using FluentAssertions;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Moq;

namespace Auctionify.UnitTests.CreateChatMessageTests
{
	public class CreateChatMessageCommandHandlerTests : IDisposable
	{
		#region Initialization

		private readonly IMapper _mapper;
		private readonly IChatMessageRepository _chatMessageRepository;
		private readonly IConversationRepository _conversationRepository;
		private readonly Mock<ICurrentUserService> _currentUserServiceMock;
		private readonly Mock<UserManager<User>> _userManagerMock;
		private readonly CreateChatMessageCommandValidator _validator;

		public CreateChatMessageCommandHandlerTests()
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

			_currentUserServiceMock.Setup(x => x.UserEmail).Returns(It.IsAny<string>());

			var newUser = new User { Id = 7 }; // User with Id = 7 is not a part of conversation with Id = 1

			_userManagerMock
				.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
				.ReturnsAsync(newUser);

			_chatMessageRepository = new ChatMessageRepository(mockDbContext.Object);
			_conversationRepository = new ConversationRepository(mockDbContext.Object);

			_validator = new CreateChatMessageCommandValidator(
				_conversationRepository,
				_userManagerMock.Object,
				_currentUserServiceMock.Object
			);
		}

		#endregion

		#region Tests

		[Fact]
		public async Task Handle_GivenValidRequest_ShouldReturnCreatedChatMessageResponse()
		{
			// Arrange
			var mapperMock = new Mock<IMapper>();
			var chatMessageRepositoryMock = new Mock<IChatMessageRepository>();
			var currentUserServiceMock = new Mock<ICurrentUserService>();
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
			var mockClientProxy = new Mock<IClientProxy>();
			var mockClients = new Mock<IHubClients>();
			mockClients
				.Setup(clients => clients.Group(It.IsAny<string>()))
				.Returns(mockClientProxy.Object);
			var mockHubContext = new Mock<IHubContext<AuctionHub>>();
			mockHubContext.Setup(x => x.Clients).Returns(mockClients.Object);

			var user = new User { Id = 1, Email = "user@example.com" };
			userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);

			var request = new CreateChatMessageCommand
			{
				ConversationId = 1,
				Body = "TestMessageText"
			};

			var handler = new CreateChatMessageCommandHandler(
				mapperMock.Object,
				chatMessageRepositoryMock.Object,
				currentUserServiceMock.Object,
				userManagerMock.Object,
				mockHubContext.Object
			);

			var sentChatMessage = new ChatMessage
			{
				Id = 1,
				SenderId = user.Id,
				ConversationId = request.ConversationId,
				Body = request.Body,
				IsRead = false,
				TimeStamp = DateTime.Now
			};

			chatMessageRepositoryMock
				.Setup(x => x.AddAsync(It.IsAny<ChatMessage>()))
				.ReturnsAsync(sentChatMessage);

			mapperMock
				.Setup(x => x.Map<CreatedChatMessageResponse>(It.IsAny<ChatMessage>()))
				.Returns(new CreatedChatMessageResponse { Id = sentChatMessage.Id });

			// Act
			var result = await handler.Handle(request, CancellationToken.None);

			// Assert
			result.Should().BeOfType<CreatedChatMessageResponse>();
			result.Should().NotBeNull();
			result.Id.Should().Be(sentChatMessage.Id);
			chatMessageRepositoryMock.Verify(x => x.AddAsync(It.IsAny<ChatMessage>()), Times.Once);
		}

		[Fact]
		public async Task Handle_GivenInvalidRequest_ShouldThrowValidationException()
		{
			// Arrange
			var request = new CreateChatMessageCommand { ConversationId = 1, Body = "" };

			// Act
			var result = await _validator.ValidateAsync(request);

			// Assert
			result.Should().BeOfType<ValidationResult>();
			result.IsValid.Should().BeFalse();
			result.Errors.Should().NotBeEmpty();
			result.Errors.Count.Should().Be(1);
			result.Errors.First().ErrorMessage.Should().Be("Body cannot be empty");
		}

		[Fact]
		public async Task Handle_GivenInvalidConversationId_ShouldThrowValidationException()
		{
			// Arrange
			var request = new CreateChatMessageCommand
			{
				ConversationId = 100,
				Body = "TestMessageText"
			};

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
				.Be("Conversation with this Id does not exist");
		}

		[Fact]
		public async Task Handle_WhenUserIsNotAPartOfConversation_ShouldThrowValidationException()
		{
			// Arrange
			var request = new CreateChatMessageCommand
			{
				ConversationId = 1,
				Body = "TestMessageText"
			};

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
				.Be("You are not a part of this conversation");
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
