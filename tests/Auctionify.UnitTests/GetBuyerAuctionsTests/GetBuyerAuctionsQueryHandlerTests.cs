using Auctionify.Application.Common.DTOs;
using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Common.Models.Requests;
using Auctionify.Application.Features.Users.Queries.GetBuyerAuctions;
using Auctionify.Core.Entities;
using Auctionify.Infrastructure.Persistence;
using Auctionify.Infrastructure.Repositories;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using User = Auctionify.Core.Entities.User;

namespace Auctionify.UnitTests.GetBuyerAuctionsTests
{
	public class GetBuyerAuctionsQueryHandlerTests : IDisposable
	{
		#region Initialization

		private readonly ILotRepository _lotRepository;
		private readonly IBidRepository _bidRepository;
		private readonly IMapper _mapper;
		private readonly Mock<IPhotoService> _photoServiceMock;
		private readonly Mock<IWatchlistService> _watchlistServiceMock;
		private readonly ICurrentUserService _currentUserService;
		private readonly UserManager<User> _userManager;

		public GetBuyerAuctionsQueryHandlerTests()
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
			_watchlistServiceMock = new Mock<IWatchlistService>();

			_lotRepository = new LotRepository(mockDbContext.Object);
			_bidRepository = new BidRepository(mockDbContext.Object);

			_userManager = EntitiesSeeding.GetUserManagerMock();
			_currentUserService = EntitiesSeeding.GetCurrentUserServiceMock();
		}

		#endregion

		#region Tests

		[Fact]
		public async Task Handle_ShouldReturnBuyerAuctions()
		{
			// Arrange
			var query = new GetBuyerAuctionsQuery
			{
				PageRequest = new PageRequest { PageIndex = 1, PageSize = 10, }
			};

			var handler = new GetBuyerAuctionsQueryHandler(
				_lotRepository,
				_mapper,
				_photoServiceMock.Object,
				_currentUserService,
				_userManager,
				_watchlistServiceMock.Object,
				_bidRepository
			);

			// Act
			var result = await handler.Handle(query, default);

			// Assert
			result.Should().NotBeNull();
			result.Should().BeOfType<GetListResponseDto<GetBuyerAuctionsResponse>>();
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
				_watchlistServiceMock.Reset();
			}
		}

		#endregion
	}
}
