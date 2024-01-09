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
		private readonly ILotRepository _lotRepository;
		private readonly IBidRepository _bidRepository;
		private readonly IMapper _mapper;
		private readonly Mock<ICurrentUserService> _currentUserServiceMock;
		private readonly Mock<IPhotoService> _photoServiceMock;
		private readonly UserManager<User> _userManager;

		public GetWatchlistLotsQueryHandlerTests()
		{
			var mockDbContext = DbContextMock.GetMock<Lot, ApplicationDbContext>(
				EntitiesSeeding.GetLots(),
				ctx => ctx.Lots
			);
			mockDbContext = DbContextMock.GetMock(
				EntitiesSeeding.GetBids(),
				ctx => ctx.Bids,
				mockDbContext
			);
			mockDbContext = DbContextMock.GetMock(
				EntitiesSeeding.GetWatchlists(),
				ctx => ctx.Watchlists,
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
			_currentUserServiceMock = new Mock<ICurrentUserService>();

			_lotRepository = new LotRepository(mockDbContext.Object);
			_bidRepository = new BidRepository(mockDbContext.Object);
			_watchlistRepository = new WatchlistRepository(mockDbContext.Object);

			_userManager = EntitiesSeeding.GetUserManagerMock();
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
				_lotRepository,
				_mapper,
				_photoServiceMock.Object,
				_currentUserServiceMock.Object,
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
				_currentUserServiceMock.Reset();
				_photoServiceMock.Reset();
			}
		}

		#endregion
	}
}
