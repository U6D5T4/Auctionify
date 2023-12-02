using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Common.Models.Requests;
using Auctionify.Application.Features.Lots.Queries.Filter;
using Auctionify.Core.Entities;
using Auctionify.Infrastructure.Persistence;
using Auctionify.Infrastructure.Repositories;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Auctionify.UnitTests.FilterLotsTests
{
	public class FilterLotsTests
	{
		#region Initialization

		private readonly IMapper _mapper;
		private readonly ILotRepository _lotRepository;
		private readonly IBidRepository _bidRepository;
		private readonly Mock<IWatchlistService> _watchListServiceMock;
		private readonly Mock<ICurrentUserService> _currentUserServiceMock;
		private readonly Mock<IPhotoService> _photoServiceMock;
		private readonly UserManager<User> _userManager;

		public FilterLotsTests()
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
						}
					)
			);

			_lotRepository = new LotRepository(mockDbContext.Object);
			_bidRepository = new BidRepository(mockDbContext.Object);
			_watchListServiceMock = new Mock<IWatchlistService>();
			_currentUserServiceMock = new Mock<ICurrentUserService>();
			_photoServiceMock = new Mock<IPhotoService>();
			_userManager = EntitiesSeeding.GetUserManagerMock();
			_currentUserServiceMock.Setup(x => x.UserEmail).Returns(It.IsAny<string>());
			_mapper = new Mapper(configuration);
		}

		#endregion

		[Fact]
		public async Task FilterLotsQueryHandler_WhenCalledWithoutParams_ReturnsAllLots()
		{
			_watchListServiceMock
				.Setup(
					x =>
						x.IsLotInUserWatchlist(
							It.IsAny<int>(),
							It.IsAny<int>(),
							It.IsAny<CancellationToken>()
						)
				)
				.ReturnsAsync(true);
			_photoServiceMock
				.Setup(x => x.GetMainPhotoUrlAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync("TestMainPhotoUrl");

			var query = new FilterLotsQuery();
			var handler = new FilterLotsQueryHandler(
				_lotRepository,
				_mapper,
				_photoServiceMock.Object,
				_currentUserServiceMock.Object,
				_userManager,
				_watchListServiceMock.Object,
				_bidRepository
			);

			var result = await handler.Handle(query, default);
			var countLots = EntitiesSeeding.GetLots().Where(l => l.LotStatus.Id != 2).Count();

			Assert.Equal(result.Count, countLots);
		}

		[Theory]
		[InlineData(0, 2)]
		[InlineData(0, 3)]
		[InlineData(0, 4)]
		public async Task FilterLotsQueryHandler_WhenCalledWithPageSizeParam_ReturnsAllLotsWithPageSize(
			int PageIndex,
			int PageSize
		)
		{
			var pageRequest = new PageRequest { PageSize = PageSize, PageIndex = PageIndex };

			var query = new FilterLotsQuery { PageRequest = pageRequest, };

			var handler = new FilterLotsQueryHandler(
				_lotRepository,
				_mapper,
				_photoServiceMock.Object,
				_currentUserServiceMock.Object,
				_userManager,
				_watchListServiceMock.Object,
				_bidRepository
			);

			var result = await handler.Handle(query, default);

			result.Items.Should().HaveCount(PageSize);
		}
	}
}
