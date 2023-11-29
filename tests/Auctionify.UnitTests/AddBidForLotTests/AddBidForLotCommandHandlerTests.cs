using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Features.Users.Commands.AddBidForLot;
using Auctionify.Application.Hubs;
using Auctionify.Core.Entities;
using Auctionify.Infrastructure.Persistence;
using Auctionify.Infrastructure.Repositories;
using AutoMapper;
using FluentAssertions;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Moq;

namespace Auctionify.UnitTests.AddBidForLotTests
{
	public class AddBidForLotCommandHandlerTests : IDisposable
	{
		#region Initialization

		private readonly IMapper _mapper;
		private readonly IBidRepository _bidRepository;
		private readonly ILotRepository _lotRepository;
		private readonly ILotStatusRepository _lotStatusRepository;
		private readonly Mock<UserManager<User>> _userManagerMock;
		private readonly Mock<ICurrentUserService> _currentUserServiceMock;
		private readonly AddBidForLotCommandValidator _validator;

		public AddBidForLotCommandHandlerTests()
		{
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
			_currentUserServiceMock = new Mock<ICurrentUserService>();
			_currentUserServiceMock.Setup(x => x.UserEmail).Returns(It.IsAny<string>());

			_bidRepository = new BidRepository(mockDbContext.Object);
			_lotRepository = new LotRepository(mockDbContext.Object);
			_lotStatusRepository = new LotStatusRepository(mockDbContext.Object);

			_validator = new AddBidForLotCommandValidator(
				_bidRepository,
				_lotRepository,
				_lotStatusRepository,
				_userManagerMock.Object,
				_currentUserServiceMock.Object
			);
		}

		#endregion

		#region Tests

		[Fact]
		public async Task Handle_WhenValidRequest_ReturnsAddedBidForLotResponse()
		{
			// Arrange
			var mapperMock = new Mock<IMapper>();
			var bidRepositoryMock = new Mock<IBidRepository>();
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
			mockClients.Setup(clients => clients.All).Returns(mockClientProxy.Object);

			var mockHubContext = new Mock<IHubContext<AuctionHub>>();
			mockHubContext.Setup(x => x.Clients).Returns(mockClients.Object);

			var handler = new AddBidForLotCommandHandler(
				mapperMock.Object,
				bidRepositoryMock.Object,
				currentUserServiceMock.Object,
				userManagerMock.Object,
				mockHubContext.Object
			);

			var command = new AddBidForLotCommand { LotId = 1, Bid = 100 };

			var user = new User { Id = 1, Email = "user@example.com" };
			userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);

			var addedBid = new Bid
			{
				Id = 1,
				BuyerId = user.Id,
				NewPrice = command.Bid,
				TimeStamp = DateTime.Now,
				LotId = command.LotId,
				BidRemoved = false
			};

			bidRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Bid>())).ReturnsAsync(addedBid);

			mapperMock
				.Setup(m => m.Map<AddedBidForLotResponse>(addedBid))
				.Returns(new AddedBidForLotResponse { Id = addedBid.Id });

			// Act
			var result = await handler.Handle(command, CancellationToken.None);

			// Assert
			result.Should().NotBeNull();
			result.Id.Should().Be(addedBid.Id);
			bidRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Bid>()), Times.Once);
		}

		[Fact]
		public async Task Handle_WhenBidIsNegative_ReturnsValidationErrors()
		{
			// Arrange
			var command = new AddBidForLotCommand { LotId = 1, Bid = -100 };

			// Act
			var result = await _validator.TestValidateAsync(command);

			// Assert
			result.Should().NotBeNull();
			result.IsValid.Should().BeFalse();
			result.Errors
				.Should()
				.Contain(
					x => x.ErrorMessage == "Bid must be greater than 0 and be a positive number"
				);
		}

		[Fact]
		public async Task Handle_WhenLotDoesNotExist_ReturnsValidationErrors()
		{
			// Arrange
			var command = new AddBidForLotCommand { LotId = 100, Bid = 100 };

			// Act
			var result = await _validator.TestValidateAsync(command);

			// Assert
			result.Should().NotBeNull();
			result.IsValid.Should().BeFalse();
			result.Errors
				.Should()
				.Contain(x => x.ErrorMessage == "Lot with this Id does not exist");
		}

		[Fact]
		public async Task Handle_WhenLotIsNotActive_ReturnsValidationErrors()
		{
			// Arrange
			var command = new AddBidForLotCommand { LotId = 2, Bid = 100 };

			// Act
			var result = await _validator.TestValidateAsync(command);

			// Assert
			result.Should().NotBeNull();
			result.IsValid.Should().BeFalse();
			result.Errors
				.Should()
				.Contain(x => x.ErrorMessage == "You can bid only if the lot is active");
		}

		[Fact]
		public async Task Handle_WhenBidIsNotGreaterThanStartingPrice_ReturnsValidationErrors()
		{
			// Arrange
			var command = new AddBidForLotCommand { LotId = 1, Bid = 50 };

			// Act
			var result = await _validator.TestValidateAsync(command);

			// Assert
			result.Should().NotBeNull();
			result.IsValid.Should().BeFalse();
			result.Errors
				.Should()
				.Contain(
					x => x.ErrorMessage == "Bid must be greater than the lot's starting price"
				);
		}

		#endregion

		#region Helpers

		public static List<LotStatus> GetLotStatuses()
		{
			return new List<LotStatus>
			{
				new LotStatus { Id = 1, Name = "Draft" },
				new LotStatus { Id = 2, Name = "PendingApproval" },
				new LotStatus { Id = 3, Name = "Rejected" },
				new LotStatus { Id = 4, Name = "Upcoming" },
				new LotStatus { Id = 5, Name = "Active" },
				new LotStatus { Id = 6, Name = "Sold" },
				new LotStatus { Id = 7, Name = "NotSold" },
				new LotStatus { Id = 8, Name = "Cancelled" },
				new LotStatus { Id = 9, Name = "Reopened" },
				new LotStatus { Id = 10, Name = "Archive" }
			};
		}

		public static List<Bid> GetBids()
		{
			return new List<Bid>
			{
				new Bid
				{
					Id = 1,
					LotId = 1,
					BuyerId = 2,
					NewPrice = 120,
					BidRemoved = false,
				},
				new Bid
				{
					Id = 2,
					LotId = 1,
					BuyerId = 3,
					NewPrice = 140,
					BidRemoved = false,
				},
				new Bid
				{
					Id = 3,
					LotId = 2,
					BuyerId = 3,
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
				new Lot
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
				new Lot
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
