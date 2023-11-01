using Auctionify.Application.Common.DTOs;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Common.Models.Requests;
using Auctionify.Application.Features.Lots.Queries.GetAll;
using Auctionify.Core.Entities;
using Auctionify.Infrastructure.Persistence;
using Auctionify.Infrastructure.Repositories;
using AutoMapper;
using FluentAssertions;

namespace Auctionify.UnitTests.GetAllLotsTests
{
    public class GetAllLotsTest
    {
        private readonly IMapper _mapper;
        private readonly ILotRepository _lotRepository;

        public GetAllLotsTest() {
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
        public async Task GetAllLotsQueryHandler_WhenCalled_ReturnsAllLotsIfAnyExists()
        {
            var allLots = EntitiesSeeding.GetLots();
            var query = new GetAllLotsQuery
            {
                PageRequest = new PageRequest
                {
                    PageIndex = 0,
                    PageSize = 10
                }
            };
            var handler = new GetAllLotsQueryHandler(_lotRepository, _mapper);

            var result = await handler.Handle(query, default);

            result.Should().BeOfType<GetListResponseDto<GetAllLotsResponse>>();
            result.Count.Should().Be(allLots.Count);
        }

        [Fact]
        public async Task GetAllLotsQueryHandler_WhenCalled_ReturnsEmptyList()
        {
            var allLots = new List<Lot>();
            var mockDbContext = DbContextMock.GetMock<Lot, ApplicationDbContext>(allLots, ctx => ctx.Lots);
            var lotRepository = new LotRepository(mockDbContext.Object);
            var query = new GetAllLotsQuery
            {
                PageRequest = new PageRequest
                {
                    PageIndex = 0,
                    PageSize = 10
                }
            };
            var handler = new GetAllLotsQueryHandler(lotRepository, _mapper);

            var result = await handler.Handle(query, default);

            result.Should().BeOfType<GetListResponseDto<GetAllLotsResponse>>();
            result.Count.Should().Be(allLots.Count);
        }

    }
}
