using Auctionify.Application.Common.DTOs;
using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Common.Models.Requests;
using Auctionify.Application.Features.Lots.Queries.GetAll;
using Auctionify.Core.Entities;
using Auctionify.Infrastructure.Persistence;
using Auctionify.Infrastructure.Repositories;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Auctionify.UnitTests.GetAllLotsTests
{
	public class GetAllLotsTests
	{
		private readonly IMapper _mapper;
		private readonly ILotRepository _lotRepository;
		private readonly IBidRepository _bidRepository;
		private readonly Mock<IWatchlistService> _watchListServiceMock;
		private readonly Mock<ICurrentUserService> _currentUserServiceMock;
		private readonly Mock<IPhotoService> _photoServiceMock;
		private readonly UserManager<User> _userManager;

		public GetAllLotsTests()
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

		[Fact]
		public async Task GetAllLotsQueryHandler_WhenCalled_ReturnsAllLotsWithNotDraftStatusIfAnyExists()
		{
			var allLots = EntitiesSeeding.GetLots();
			var query = new GetAllLotsQuery
			{
				PageRequest = new PageRequest { PageIndex = 0, PageSize = 10 }
			};

			var handler = new GetAllLotsQueryHandler(
				_lotRepository,
				_mapper,
				_photoServiceMock.Object,
				_currentUserServiceMock.Object,
				_userManager,
				_watchListServiceMock.Object,
				_bidRepository
			);

			var result = await handler.Handle(query, default);

			result.Should().BeOfType<GetListResponseDto<GetAllLotsResponse>>();
			var countLots = allLots.Where(l => l.LotStatus.Id != 2).Count();

			result.Count.Should().Be(countLots);
		}

		[Fact]
		public async Task GetAllLotsQueryHandler_WhenCalled_ReturnsEmptyList()
		{
			var allLots = new List<Lot>();
			var mockDbContext = DbContextMock.GetMock<Lot, ApplicationDbContext>(
				allLots,
				ctx => ctx.Lots
			);
			var lotRepository = new LotRepository(mockDbContext.Object);
			var query = new GetAllLotsQuery
			{
				PageRequest = new PageRequest { PageIndex = 0, PageSize = 10 }
			};
			var handler = new GetAllLotsQueryHandler(
				lotRepository,
				_mapper,
				_photoServiceMock.Object,
				_currentUserServiceMock.Object,
				_userManager,
				_watchListServiceMock.Object,
				_bidRepository
			);

			var result = await handler.Handle(query, default);

			result.Should().BeOfType<GetListResponseDto<GetAllLotsResponse>>();
			result.Count.Should().Be(allLots.Count);
		}
	}
}
