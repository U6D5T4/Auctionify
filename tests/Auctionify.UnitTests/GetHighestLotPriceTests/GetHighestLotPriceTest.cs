using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Features.Lots.Queries.GetHighestLotPrice;
using Auctionify.Core.Entities;
using Auctionify.Infrastructure.Persistence;
using Auctionify.Infrastructure.Repositories;
using FluentAssertions;

namespace Auctionify.UnitTests.GetHighestLotPriceTests
{
	public class GetHighestLotPriceTest
	{
		ILotRepository _lotRepository;
		IBidRepository _bidRepository;

		public GetHighestLotPriceTest()
		{
			var mockDbContext = DbContextMock.GetMock<Lot, ApplicationDbContext>(EntitiesSeeding.GetLots(), ctx => ctx.Lots);
			mockDbContext = DbContextMock.GetMock(EntitiesSeeding.GetBids(), ctx => ctx.Bids, mockDbContext);

			_lotRepository = new LotRepository(mockDbContext.Object);
			_bidRepository = new BidRepository(mockDbContext.Object);
		}

		[Fact]
		public async Task GetHighestLotPriceQueryHandler_WhenCalledWithLotsAndBidsInSystem_ReturnsHighestPrice()
		{
			var query = new GetHighestLotPriceQuery();

			var handler = new GetHighestLotPriceQueryHandler(_lotRepository, _bidRepository);

			var result = await handler.Handle(query, default);

			result.Should().BeOfType(typeof(decimal));
			result.Should().Be(140);
		}
	}
}
