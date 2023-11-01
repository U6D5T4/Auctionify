using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Features.Lots.Queries.GetById;
using Auctionify.Core.Entities;
using Auctionify.Infrastructure.Persistence;
using Auctionify.Infrastructure.Repositories;
using AutoMapper;
using FluentAssertions;

namespace Auctionify.UnitTests.GetLotByIdTests
{
    public class GetLotByIdTests
    {
        private readonly IMapper _mapper;
        private readonly ILotRepository _lotRepository;

        public GetLotByIdTests()
        {
            var mockDbContext = DbContextMock.GetMock<Lot, ApplicationDbContext>(EntitiesSeeding.GetLots(), ctx => ctx.Lots);
            var configuration = new MapperConfiguration(cfg => cfg.AddProfiles(new List<Profile>
            {
                new Application.Common.Profiles.MappingProfiles(),
                new Application.Features.Lots.Profiles.MappingProfiles(),
            }));

            _lotRepository = new LotRepository(mockDbContext.Object);
            _mapper = new Mapper(configuration);
        }

        [Fact]
        public async Task GetByIdLotQueryHandler_WhenCalledWithCorrectId_ReturnsLot()
        {
            var query = new GetByIdLotQuery
            {
                Id = 1,
            };

            var handler = new GetByIdLotQueryHandler(_lotRepository, _mapper);

            var result = await handler.Handle(query, default);

            result.Should().BeOfType<GetByIdLotResponse>();
            result.Id.Should().Be(query.Id);
        }
    }
}
