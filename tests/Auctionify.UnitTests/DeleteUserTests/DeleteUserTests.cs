using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Features.Users.Commands.Delete;
using Auctionify.Core.Entities;
using Auctionify.Core.Enums;
using Auctionify.Infrastructure.Persistence;
using Auctionify.Infrastructure.Repositories;
using FluentAssertions;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Identity;
using MockQueryable.Moq;
using Moq;

namespace Auctionify.UnitTests.DeleteUserTests
{
	public class DeleteUserTests
	{
		#region Initialization

		private readonly UserManager<User> _userManager;
		private readonly ICurrentUserService _currentUserService;
		private readonly IBidRepository _bidRepository;
		private readonly ILotRepository _lotRepository;
		private readonly ILotStatusRepository _lotStatusRepository;
		private readonly DeleteUserCommandValidator _validator;

		public DeleteUserTests()
		{
			_userManager = EntitiesSeeding.GetUserManagerMock();
			_currentUserService = EntitiesSeeding.GetCurrentUserServiceMock();

			var mockDbContext = DbContextMock.GetMock<Bid, ApplicationDbContext>(
				GetBids(),
				ctx => ctx.Bids
			);

			mockDbContext = DbContextMock.GetMock(
				GetLotStatuses(),
				ctx => ctx.LotStatuses,
				mockDbContext
			);

			mockDbContext = DbContextMock.GetMock(GetLots(), ctx => ctx.Lots, mockDbContext);

			_bidRepository = new BidRepository(mockDbContext.Object);
			_lotRepository = new LotRepository(mockDbContext.Object);
			_lotStatusRepository = new LotStatusRepository(mockDbContext.Object);

			_validator = new DeleteUserCommandValidator(
				_userManager,
				_currentUserService,
				_bidRepository,
				_lotRepository,
				_lotStatusRepository
			);
		}

		#endregion

		#region Tests

		[Fact]
		public async Task DeleteUserCommand_DeletesUserSuccessfully()
		{
			var command = new DeleteUserCommand();

			var handler = new DeleteUserCommandHandler(_userManager, _currentUserService);

			var result = await handler.Handle(command, CancellationToken.None);

			result.Should().BeOfType<DeletedUserResponse>();
			result.IsDeleted.Should().BeTrue();
			result.Message.Should().Be("User deleted successfully");
		}

		[Fact]
		public async Task DeleteUserCommand_WhenUserIsBuyerAndHasAtLeastOneActiveBid_ReturnsValidationErrors()
		{
			// Arrange
			var command = new DeleteUserCommand();

			// Act
			var result = await _validator.TestValidateAsync(command);

			// Assert
			result.Should().NotBeNull();
			result.IsValid.Should().BeFalse();
			result.Errors.Should().HaveCount(1);
			result.Errors[0].ErrorMessage.Should().Be("You can't delete your account");
		}

		[Fact]
		public async Task DeleteUserCommand_WhenUserIsSellerAndHasAtLeastOneLotWithEitherActiveUpcomingPendingApprovalOrReopenedStatus_ReturnsValidationErrors()
		{
			// Arrange
			var command = new DeleteUserCommand();
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
				Email = "test@test.COM",
				IsDeleted = false
			};

			var mock = EntitiesSeeding.GetUsers().AsQueryable().BuildMockDbSet();
			userManagerMock.Setup(m => m.Users).Returns(mock.Object);

			var roles = new List<string> { UserRole.Seller.ToString() };
			userManagerMock.Setup(m => m.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(roles);

			var currentUserServiceMock = new Mock<ICurrentUserService>();
			currentUserServiceMock.Setup(m => m.UserEmail).Returns(user.Email);

			var validator = new DeleteUserCommandValidator(
				userManagerMock.Object,
				currentUserServiceMock.Object,
				_bidRepository,
				_lotRepository,
				_lotStatusRepository
			);

			// Act
			var result = await validator.TestValidateAsync(command);

			// Assert
			result.Should().NotBeNull();
			result.IsValid.Should().BeFalse();
			result.Errors.Should().HaveCount(1);
			result.Errors[0].ErrorMessage.Should().Be("You can't delete your account");
		}

		#endregion

		#region Helpers

		public static List<LotStatus> GetLotStatuses()
		{
			return new List<LotStatus>
			{
				new() { Id = 1, Name = "Draft" },
				new() { Id = 2, Name = "PendingApproval" },
				new() { Id = 3, Name = "Rejected" },
				new() { Id = 4, Name = "Upcoming" },
				new() { Id = 5, Name = "Active" },
				new() { Id = 6, Name = "Sold" },
				new() { Id = 7, Name = "NotSold" },
				new() { Id = 8, Name = "Cancelled" },
				new() { Id = 9, Name = "Reopened" },
				new() { Id = 10, Name = "Archive" }
			};
		}

		public static List<Bid> GetBids()
		{
			return new List<Bid>
			{
				new()
				{
					Id = 1,
					LotId = 1,
					BuyerId = 1,
					NewPrice = 120,
					BidRemoved = false,
				},
				new()
				{
					Id = 2,
					LotId = 1,
					BuyerId = 1,
					NewPrice = 140,
					BidRemoved = false,
				},
				new()
				{
					Id = 3,
					LotId = 2,
					BuyerId = 1,
					NewPrice = 160,
					BidRemoved = false,
				}
			};
		}

		public static List<Lot> GetLots()
		{
			var bids = GetBids();
			var lotStatuses = GetLotStatuses();

			return new List<Lot>
			{
				new()
				{
					Id = 1,
					Title = "Test Lot Title",
					Description =
						"Test Lot Description - Test Lot Description - Test Lot Description",
					LotStatusId = 5,
					LotStatus = lotStatuses.Find(x => x.Id == 5)!,
					StartDate = DateTime.Now,
					EndDate = DateTime.Now.AddDays(1),
					Location = new Location
					{
						Address = "Test Address",
						City = "Test City",
						Country = "Test Country",
					},
					StartingPrice = 100,
					SellerId = 1,
					CategoryId = 1,
					CurrencyId = 1,
					Bids = new List<Bid> { bids[0], bids[1], }
				},
				new()
				{
					Id = 2,
					Title = "Test Lot Title 2",
					Description =
						"Test Lot Description - Test Lot Description - Test Lot Description",
					LotStatusId = 6,
					LotStatus = lotStatuses.Find(x => x.Id == 6)!,
					StartDate = DateTime.Now,
					EndDate = DateTime.Now.AddDays(1),
					Location = new Location
					{
						Address = "Test Address",
						City = "Test City",
						Country = "Test Country",
					},
					StartingPrice = 100,
					SellerId = 1,
					CategoryId = 1,
					CurrencyId = 1,
					Bids = new List<Bid> { bids[2], }
				}
			};
		}

		#endregion
	}
}
