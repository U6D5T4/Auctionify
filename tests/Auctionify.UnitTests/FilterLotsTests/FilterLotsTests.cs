using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Common.Models.Requests;
using Auctionify.Application.Features.Lots.Queries.FIlter;
using Auctionify.Core.Entities;
using Auctionify.Infrastructure.Persistence;
using Auctionify.Infrastructure.Repositories;
using AutoMapper;
using FluentAssertions;
using Moq;

namespace Auctionify.UnitTests.FilterLotsTests
{
    public class FilterLotsTests
    {
        private ILotRepository _repository;
        private IMapper _mapper;

        public FilterLotsTests()
        {
            var mockDbContext = DbContextMock.GetMock<Lot, ApplicationDbContext>(GetAllLots(), ctx => ctx.Lots);

            _repository = new LotRepository(mockDbContext.Object);

            var configuration = new MapperConfiguration(cfg => cfg.AddProfiles(new List<Profile>
            {
                new Application.Common.Profiles.MappingProfiles(),
                new Application.Features.Lots.Profiles.MappingProfiles(),
            }));
            _mapper = new Mapper(configuration);
        }

        [Fact]
        public async Task FilterLotsQueryHandler_WhenCalledWithoutParams_ReturnsAllLots()
        {
            var query = new FilterLotsQuery();
            var handler = new FilterLotsQueryHandler(_repository, _mapper);

            var result = await handler.Handle(query, default);

            Assert.Equal(result.Count, GetAllLots().Count);
        }

        [Theory]
        [InlineData(0, 2)]
        [InlineData(0, 3)]
        [InlineData(0, 4)]
        public async Task FilterLotsQueryHandler_WhenCalledWithPageSizeParam_ReturnsAllLotsWithPageSize(int PageIndex, int PageSize)
        {
            var pageRequest = new PageRequest
            {
                PageSize = PageSize,
                PageIndex = PageIndex
            };

            var query = new FilterLotsQuery
            {
                PageRequest = pageRequest,
            };

            var handler = new FilterLotsQueryHandler(_repository, _mapper);

            var result = await handler.Handle(query, default);

            result.Items.Should().HaveCount(PageSize);
        }

        private List<Lot> GetAllLots()
        {
            return new List<Lot>
            {
                new Lot
                {
                    Id = 1,
                    Title = "asdasd",
                    CategoryId = 1,
                },
                new Lot
                {
                    Id = 2,
                    Title = "asdasdsa",
                    CategoryId = 1,
                },
                new Lot
                {
                    Id = 3,
                    Title = "asdasdsa",
                    CategoryId = 2,
                },
                new Lot
                {
                    Id = 4,
                    Title = "asdasdsa",
                    CategoryId = 3,
                },
                new Lot
                {
                    Id = 5,
                    Title = "asdasdsa",
                    CategoryId = 2,
                }
            };
        }
    }
}
