using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Features.Lots.Commands.Delete;
using Auctionify.Core.Entities;
using Auctionify.Infrastructure.Persistence;
using Auctionify.Infrastructure.Repositories;
using AutoMapper;
using FluentAssertions;
using Moq;

namespace Auctionify.UnitTests.DeleteLotTests
{
    public class DeleteLotTests
    {
        private readonly ILotRepository _lotRepository;
        private readonly IMapper _mapper;
        private readonly ILotStatusRepository _lotStatusRepository;
        private readonly Mock<IBidRepository> _bidRepository;


        public DeleteLotTests()
        {
            var mockDbContext = DbContextMock.GetMock<LotStatus, ApplicationDbContext>(EntitiesSeeding.GetLotStatuses(), ctx => ctx.LotStatuses);
            mockDbContext = DbContextMock.GetMock(EntitiesSeeding.GetCategories(), ctx => ctx.Categories, mockDbContext);
            mockDbContext = DbContextMock.GetMock(EntitiesSeeding.GetCurrencies(), ctx => ctx.Currency, mockDbContext);
            mockDbContext = DbContextMock.GetMock(EntitiesSeeding.GetBids(), ctx => ctx.Bids, mockDbContext);
            mockDbContext = DbContextMock.GetMock(EntitiesSeeding.GetLots(), ctx => ctx.Lots, mockDbContext);


            var configuration = new MapperConfiguration(cfg => cfg.AddProfiles(new List<Profile>
            {
                new Application.Common.Profiles.MappingProfiles(),
                new Application.Features.Lots.Profiles.MappingProfiles(),
            }));
            _mapper = new Mapper(configuration);

            _lotRepository = new LotRepository(mockDbContext.Object);
            _lotStatusRepository = new LotStatusRepository(mockDbContext.Object);
            var bidRepository = new Mock<IBidRepository>();
            bidRepository.CallBase = true;

            _bidRepository = bidRepository;
        }

        [Fact]
        public async Task DeleteLotCommandHandler_WhenCalledWithActiveLotId_ChangesStatusToCancelledAndDeletesBids()
        {
            var cmd = new DeleteLotCommand { Id = 1 };

            var handler = new DeleteLotCommandHandler(_mapper, _lotRepository, _lotStatusRepository, _bidRepository.Object);

            var result = await handler.Handle(cmd, default);

            result.Should().BeOfType<DeletedLotResponse>();
            result.WasDeleted.Should().BeFalse();
            result.Id.Should().Be(cmd.Id);
            _bidRepository.Verify(x => x.DeleteRangeAsync(It.IsAny<ICollection<Bid>>(), false), Times.Once());
        }

        [Fact]
        public async Task DeleteLotCommandHandler_WhenCalledWithNotActiveStatusLotId_DeletesLot()
        {
            var cmd = new DeleteLotCommand { Id = 2 };

            var handler = new DeleteLotCommandHandler(_mapper, _lotRepository, _lotStatusRepository, _bidRepository.Object);

            var result = await handler.Handle(cmd, default);

            result.Should().BeOfType<DeletedLotResponse>();
            result.WasDeleted.Should().BeTrue();
            result.Id.Should().Be(cmd.Id);
        }
    }
}
