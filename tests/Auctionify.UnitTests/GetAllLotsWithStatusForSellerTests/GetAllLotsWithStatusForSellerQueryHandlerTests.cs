using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Common.Interfaces;
using Auctionify.Core.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moq;
using Auctionify.Infrastructure.Persistence;
using Auctionify.Infrastructure.Repositories;
using Auctionify.Application.Features.Lots.Queries.GetAllLotsWithStatusForSeller;
using Auctionify.Application.Common.Models.Requests;
using Auctionify.Application.Common.DTOs;
using FluentAssertions;
using Auctionify.Core.Enums;

namespace Auctionify.UnitTests.GetAllLotsWithStatusForSellerTests
{
	public class GetAllLotsWithStatusForSellerQueryHandlerTests
	{
		#region Initialization

		private readonly IMapper _mapper;
		private readonly ILotRepository _lotRepository;
		private readonly Mock<ICurrentUserService> _currentUserServiceMock;
		private readonly UserManager<User> _userManager;
		private readonly Mock<IPhotoService> _photoServiceMock;

		public GetAllLotsWithStatusForSellerQueryHandlerTests()
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

			mockDbContext = DbContextMock.GetMock(
				GetCurrency(),
				ctx => ctx.Currency,
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
							new Application.Features.Lots.Profiles.MappingProfiles(),
						}
					)
			);

			_mapper = new Mapper(configuration);

			_userManager = EntitiesSeeding.GetUserManagerMock();
			_currentUserServiceMock = new Mock<ICurrentUserService>();
			_currentUserServiceMock.Setup(x => x.UserEmail).Returns(It.IsAny<string>());

			_photoServiceMock = new Mock<IPhotoService>();
			_photoServiceMock.Setup(x => x.GetMainPhotoUrlAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(It.IsAny<string>());

			_lotRepository = new LotRepository(mockDbContext.Object);
		}

		#endregion

		#region Tests

		[Fact]
		public async Task GetAllLotsWithStatusForSellerQueryQueryHandler_WhenCalledWithActiveLotStatus_ReturnsAllActiveLotsOfSeller()
		{
			var query = new GetAllLotsWithStatusForSellerQuery
			{
				LotStatus = AuctionStatus.Active,
				PageRequest = new PageRequest { PageIndex = 0, PageSize = 10 }
			};

			var handler = new GetAllLotsWithStatusForSellerQueryHandler(
				_lotRepository,
				_mapper,
				_currentUserServiceMock.Object,
				_userManager,
				_photoServiceMock.Object);

			var result = await handler.Handle(query, default);

			result.Should().BeOfType<GetListResponseDto<GetAllLotsWithStatusForSellerResponse>>();
			result.Should().NotBeNull();
			result.Items.Should().OnlyContain(x => x.LotStatus.Name == AuctionStatus.Active.ToString());
		}

		[Fact]
		public async Task GetAllLotsWithStatusForSellerQueryQueryHandler_WhenCalledWithDraftLotStatus_ReturnsAllDraftLotsOfSeller()
		{
			var query = new GetAllLotsWithStatusForSellerQuery
			{
				LotStatus = AuctionStatus.Draft,
				PageRequest = new PageRequest { PageIndex = 0, PageSize = 10 }
			};

			var handler = new GetAllLotsWithStatusForSellerQueryHandler(
				_lotRepository,
				_mapper,
				_currentUserServiceMock.Object,
				_userManager,
				_photoServiceMock.Object);

			var result = await handler.Handle(query, default);

			result.Should().BeOfType<GetListResponseDto<GetAllLotsWithStatusForSellerResponse>>();
			result.Should().NotBeNull();
			result.Items.Should().OnlyContain(x => x.LotStatus.Name == AuctionStatus.Draft.ToString());
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

		public static List<Currency> GetCurrency()
		{
			return new List<Currency>
			{
				new Currency { Id = 1, Code = "USD" },
				new Currency { Id = 2, Code = "RUB" }
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
					Currency = new Currency { Id = 1, Code = "USD" },
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
					Currency = new Currency { Id = 1, Code = "USD" },
					Bids = new List<Bid> { bids[2], }
				},
				new Lot
				{
					Id = 3,
					Title = "Test Lot Title",
					Description =
						"Test Lot Description - Test Lot Description - Test Lot Description",
					LotStatusId = 1,
					LotStatus = lotStatuses.Find(x => x.Id == 1)!,
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
					Currency = new Currency { Id = 1, Code = "USD" },
				},
			};
		}

		#endregion
	}
}
