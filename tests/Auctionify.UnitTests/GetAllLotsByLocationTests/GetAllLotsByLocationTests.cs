using Auctionify.Application.Common.DTOs;
using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Common.Models.Requests;
using Auctionify.Application.Features.Lots.Queries.GetAllByName;
using Auctionify.Core.Entities;
using Auctionify.Infrastructure.Persistence;
using Auctionify.Infrastructure.Repositories;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Auctionify.UnitTests.GetAllLotsByLocationTests
{
	public class GetAllLotsByLocationTests
	{
		private readonly IMapper _mapper;
		private readonly ILotRepository _lotRepository;
		private readonly IBidRepository _bidRepository;
		private readonly Mock<IWatchlistService> _watchListServiceMock;
		private readonly Mock<ICurrentUserService> _currentUserServiceMock;
		private readonly Mock<IPhotoService> _photoServiceMock;
		private readonly UserManager<User> _userManager;

		public GetAllLotsByLocationTests()
		{
			var mockDbContext = DbContextMock.GetMock<Lot, ApplicationDbContext>(
				EntitiesSeeding.GetLots(),
				ctx => ctx.Lots
			);
			mockDbContext = DbContextMock.GetMock(
				EntitiesSeeding.GetFiles(),
				ctx => ctx.Files,
				mockDbContext
			);
			mockDbContext = DbContextMock.GetMock(
				EntitiesSeeding.GetLotStatuses(),
				ctx => ctx.LotStatuses,
				mockDbContext
			);
			mockDbContext = DbContextMock.GetMock(
				EntitiesSeeding.GetBids(),
				ctx => ctx.Bids,
				mockDbContext
			);

			var configuration = new MapperConfiguration(
				cfg =>
					cfg.AddProfiles(
						new List<Profile>
						{
							new Application.Common.Profiles.MappingProfiles(),
							new Application.Features.Lots.Profiles.MappingProfiles(),
						}
					)
			);

			_watchListServiceMock = new Mock<IWatchlistService>();
			_photoServiceMock = new Mock<IPhotoService>();
			_currentUserServiceMock = new Mock<ICurrentUserService>();

			_lotRepository = new LotRepository(mockDbContext.Object);
			_bidRepository = new BidRepository(mockDbContext.Object);

			_userManager = EntitiesSeeding.GetUserManagerMock();

			_mapper = new Mapper(configuration);
		}

		[Fact]
		public async Task GetAllLotsByLocationQueryHandler_WhenNoLotsMatchLocation_ReturnsEmptyList()
		{
			// Arrange
			var query = new GetAllLotsByLocationQuery
			{
				PageRequest = new PageRequest { PageIndex = 0, PageSize = 10 },
				Location = "NonExistentLocation"
			};

			var handler = new GetAllLotsByLocationQueryHandler(
				_lotRepository,
				_currentUserServiceMock.Object,
				_userManager,
				_watchListServiceMock.Object,
				_photoServiceMock.Object,
				_mapper,
				_bidRepository
			);

			// Act
			var result = await handler.Handle(query, default);

			// Assert
			result.Should().BeOfType<GetListResponseDto<GetAllLotsByLocationResponse>>();
			result.Count.Should().Be(0);
		}


		// wrong test case... no need to check watchlist info.. 
		//[Fact]
		//public async Task GetAllLotsByLocationQueryHandler_WhenNoUser_ReturnsLotsWithoutWatchlistInfo()
		//{
		//	var query = new GetAllLotsByLocationQuery
		//	{
		//		PageRequest = new PageRequest { PageIndex = 0, PageSize = 10 },
		//		Location = "Test"
		//	};

		//	var handler = new GetAllLotsByLocationQueryHandler(
		//		_lotRepository,
		//		_currentUserServiceMock.Object,
		//		_userManager,
		//		_watchListServiceMock.Object,
		//		_photoServiceMock.Object,
		//		_mapper,
		//		_bidRepository
		//	);

		//	var result = await handler.Handle(query, default);

		//	result.Should().BeOfType<GetListResponseDto<GetAllLotsByLocationResponse>>();
		//	result.Count.Should().Be(1);
		//}

		// the case is wrong.. the tests passes no because of pageSize is 0, but because of the location is invalid
		//[Fact]
		//public async Task GetAllLotsByLocationQueryHandler_WhenPageSizeIsZero_ReturnsEmptyList()
		//{
		//	// Arrange
		//	var query = new GetAllLotsByLocationQuery
		//	{
		//		PageRequest = new PageRequest { PageIndex = 0, PageSize = 0 },
		//		Location = "London"
		//	};

		//	var handler = new GetAllLotsByLocationQueryHandler(
		//		_lotRepository,
		//		_currentUserServiceMock.Object,
		//		_userManager,
		//		_watchListServiceMock.Object,
		//		_photoServiceMock.Object,
		//		_mapper,
		//		_bidRepository
		//	);

		//	// Act
		//	var result = await handler.Handle(query, default);

		//	// Assert
		//	result.Should().BeOfType<GetListResponseDto<GetAllLotsByLocationResponse>>();
		//	result.Count.Should().Be(0);
		//}

		// you already checked that the location is invalid in the first test case..
		// write some other test case
		//[Fact]
		//public async Task GetAllLotsByLocationQueryHandler_WhenInvalidLocation_ReturnsEmptyList()
		//{
		//	var query = new GetAllLotsByLocationQuery
		//	{
		//		PageRequest = new PageRequest { PageIndex = 0, PageSize = 10 },
		//		Location = "InvalidLocation"
		//	};

		//	var handler = new GetAllLotsByLocationQueryHandler(
		//		_lotRepository,
		//		_currentUserServiceMock.Object,
		//		_userManager,
		//		_watchListServiceMock.Object,
		//		_photoServiceMock.Object,
		//		_mapper,
		//		_bidRepository
		//	);

		//	var result = await handler.Handle(query, default);

		//	result.Should().BeOfType<GetListResponseDto<GetAllLotsByLocationResponse>>();
		//	result.Count.Should().Be(0);
		//}
	}
}
