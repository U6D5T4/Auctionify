using Auctionify.Application.Common.DTOs;
using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Common.Models.Requests;
using Auctionify.Application.Features.Users.Queries.GetByUserWatchlist;
using Auctionify.Core.Entities;
using Auctionify.Infrastructure.Persistence;
using Auctionify.Infrastructure.Repositories;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Auctionify.UnitTests.GetWatshlistLotsTests
{
	public class GetWatchlistLotsQueryHandlerTests : IDisposable
	{
		#region Initialization

		private readonly IWatchlistRepository _watchlistRepository;
		private readonly ILocationRepository _locationRepository;
		private readonly ICategoryRepository _categoryRepository;
		private readonly ICurrencyRepository _currencyRepository;
		private readonly ILotStatusRepository _lotStatusRepository;
		private readonly IBidRepository _bidRepository;
		private readonly IMapper _mapper;
		private readonly Mock<IPhotoService> _photoServiceMock;
		private readonly UserManager<User> _userManager;
		private readonly ICurrentUserService _currentUserService;

		public GetWatchlistLotsQueryHandlerTests()
		{
			var mockDbContext = DbContextMock.GetMock<Watchlist, ApplicationDbContext>(
				EntitiesSeeding.GetWatchlists(),
				ctx => ctx.Watchlists
			);
			mockDbContext = DbContextMock.GetMock(
				EntitiesSeeding.GetBids(),
				ctx => ctx.Bids,
				mockDbContext
			);
			mockDbContext = DbContextMock.GetMock(
				EntitiesSeeding.GetLotStatuses(),
				ctx => ctx.LotStatuses,
				mockDbContext
			);
			mockDbContext = DbContextMock.GetMock(
				EntitiesSeeding.GetCategories(),
				ctx => ctx.Categories,
				mockDbContext
			);
			mockDbContext = DbContextMock.GetMock(
				EntitiesSeeding.GetCurrencies(),
				ctx => ctx.Currency,
				mockDbContext
			);
			mockDbContext = DbContextMock.GetMock(
				EntitiesSeeding.GetLocations(),
				ctx => ctx.Locations,
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
						}
					)
			);

			_mapper = new Mapper(configuration);

			_photoServiceMock = new Mock<IPhotoService>();

			_bidRepository = new BidRepository(mockDbContext.Object);
			_watchlistRepository = new WatchlistRepository(mockDbContext.Object);
			_locationRepository = new LocationRepository(mockDbContext.Object);
			_categoryRepository = new CategoryRepository(mockDbContext.Object);
			_currencyRepository = new CurrencyRepository(mockDbContext.Object);
			_lotStatusRepository = new LotStatusRepository(mockDbContext.Object);

			_userManager = EntitiesSeeding.GetUserManagerMock();
			_currentUserService = EntitiesSeeding.GetCurrentUserServiceMock();
		}

		#endregion

		#region Tests

		[Fact]
		public async Task Handle_ShouldReturnWatchlistLots()
		{
			// Arrange
			var query = new GetWatchlistLotsQuery
			{
				PageRequest = new PageRequest { PageIndex = 1, PageSize = 10 }
			};

			var handler = new GetWatchlistLotsQueryHandler(
				_watchlistRepository,
				_locationRepository,
				_categoryRepository,
				_currencyRepository,
				_lotStatusRepository,
				_mapper,
				_photoServiceMock.Object,
				_currentUserService,
				_userManager,
				_bidRepository
			);

			// Act
			var result = await handler.Handle(query, default);

			// Assert
			result.Should().BeOfType<GetListResponseDto<GetWatchlistLotsResponse>>();
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
				_photoServiceMock.Reset();
			}
		}

		#endregion
	}
}
