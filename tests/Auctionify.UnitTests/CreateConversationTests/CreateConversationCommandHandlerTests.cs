using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Features.Chats.Commands.CreateConversation;
using Auctionify.Core.Entities;
using Auctionify.Core.Enums;
using Auctionify.Infrastructure.Persistence;
using Auctionify.Infrastructure.Repositories;
using AutoMapper;
using FluentAssertions;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Auctionify.UnitTests.CreateConversationTests
{
	public class CreateConversationCommandHandlerTests : IDisposable
	{
		#region Initialization

		private readonly IMapper _mapper;
		private readonly ILotRepository _lotRepository;
		private readonly ILotStatusRepository _lotStatusRepository;
		private readonly IConversationRepository _conversationRepository;
		private readonly Mock<ICurrentUserService> _currentUserServiceMock;
		private readonly Mock<UserManager<User>> _userManagerMock;
		private readonly CreateConversationCommandValidator _validator;

		public CreateConversationCommandHandlerTests()
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

			mockDbContext = DbContextMock.GetMock(
				EntitiesSeeding.GetLots(),
				ctx => ctx.Lots,
				mockDbContext
			);

			mockDbContext = DbContextMock.GetMock(
				EntitiesSeeding.GetLotStatuses(),
				ctx => ctx.LotStatuses,
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

			var newUser = new User { Id = 1 };
			var roles = new List<string> { UserRole.Buyer.ToString() };

			_userManagerMock
				.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
				.ReturnsAsync(newUser);

			_userManagerMock.Setup(m => m.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(roles);

			_lotRepository = new LotRepository(mockDbContext.Object);
			_conversationRepository = new ConversationRepository(mockDbContext.Object);
			_lotStatusRepository = new LotStatusRepository(mockDbContext.Object);

			_validator = new CreateConversationCommandValidator(
				_lotRepository,
				_lotStatusRepository,
				_currentUserServiceMock.Object,
				_userManagerMock.Object
			);
		}

		#endregion

		#region Tests

		[Fact]
		public async Task Handle_GivenValidRequest_ShouldReturnCreatedConversationResponse()
		{
			// Arrange
			var sut = new CreateConversationCommandHandler(
				_mapper,
				_lotRepository,
				_conversationRepository,
				_currentUserServiceMock.Object,
				_userManagerMock.Object
			);

			var request = new CreateConversationCommand { LotId = 1 };

			// Act
			var result = await sut.Handle(request, CancellationToken.None);

			// Assert
			result.Should().BeOfType<CreatedConversationResponse>();
		}

		[Fact]
		public async Task Handle_GivenInvalidLotId_ShouldThrowValidationException()
		{
			// Arrange
			var request = new CreateConversationCommand { LotId = 100 };

			// Act
			var result = await _validator.ValidateAsync(request);

			// Assert
			result.Should().BeOfType<ValidationResult>();
			result.IsValid.Should().BeFalse();
			result.Errors.Should().NotBeEmpty();
			result.Errors.Count.Should().Be(1);
			result.Errors.First().ErrorMessage.Should().Be("Lot with this Id does not exist");
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
