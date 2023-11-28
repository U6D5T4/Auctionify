using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Common.Options;
using Auctionify.Application.Features.Lots.Commands.Delete;
using Auctionify.Core.Entities;
using Auctionify.Infrastructure.Persistence;
using Auctionify.Infrastructure.Repositories;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;

namespace Auctionify.UnitTests.DeleteLotTests
{
    public class DeleteLotTests
    {
        private readonly ILotRepository _lotRepository;
        private readonly IMapper _mapper;
        private readonly ILotStatusRepository _lotStatusRepository;
        private readonly IFileRepository _fileRepository;
        private readonly Mock<IBidRepository> _bidRepository;
        private readonly Mock<IBlobService> _blobServiceMock;
		private readonly Mock<IJobSchedulerService> _jobSchedulerServiceMock;
		private readonly Mock<IOptions<AzureBlobStorageOptions>> _blobStorageOptionsMock;

        public DeleteLotTests()
        {
            var mockDbContext = DbContextMock.GetMock<LotStatus, ApplicationDbContext>(EntitiesSeeding.GetLotStatuses(), ctx => ctx.LotStatuses);
            mockDbContext = DbContextMock.GetMock(EntitiesSeeding.GetCategories(), ctx => ctx.Categories, mockDbContext);
            mockDbContext = DbContextMock.GetMock(EntitiesSeeding.GetCurrencies(), ctx => ctx.Currency, mockDbContext);
            mockDbContext = DbContextMock.GetMock(EntitiesSeeding.GetBids(), ctx => ctx.Bids, mockDbContext);
            mockDbContext = DbContextMock.GetMock(EntitiesSeeding.GetLots(), ctx => ctx.Lots, mockDbContext);
            mockDbContext = DbContextMock.GetMock(EntitiesSeeding.GetFiles(), ctx => ctx.Files, mockDbContext);
			var blobStorageOptionsMock = new Mock<IOptions<AzureBlobStorageOptions>>();

			var configuration = new MapperConfiguration(cfg => cfg.AddProfiles(new List<Profile>
            {
                new Application.Common.Profiles.MappingProfiles(),
                new Application.Features.Lots.Profiles.MappingProfiles(),
            }));
            _mapper = new Mapper(configuration);

			blobStorageOptionsMock.Setup(x => x.Value).Returns(new AzureBlobStorageOptions
			{
				ContainerName = "auctionify-files",
				PhotosFolderName = "photos",
				AdditionalDocumentsFolderName = "additional-documents"
			});

			_blobStorageOptionsMock = blobStorageOptionsMock;
			_lotRepository = new LotRepository(mockDbContext.Object);
            _lotStatusRepository = new LotStatusRepository(mockDbContext.Object);
            _fileRepository = new FileRepository(mockDbContext.Object);
            var bidRepository = new Mock<IBidRepository>();
			var jobSchedulerService = new Mock<IJobSchedulerService>();

			bidRepository.CallBase = true;
            _blobServiceMock = new Mock<IBlobService>();
			_jobSchedulerServiceMock = jobSchedulerService;

			_bidRepository = bidRepository;
        }

        [Fact]
        public async Task DeleteLotCommandHandler_WhenCalledWithActiveLotId_ChangesStatusToCancelledAndDeletesBids()
        {
            var cmd = new DeleteLotCommand { Id = 1 };

            var handler = new DeleteLotCommandHandler(
                _mapper,
                _lotRepository,
                _lotStatusRepository,
                _bidRepository.Object,
                _fileRepository,
                _blobServiceMock.Object,
                _blobStorageOptionsMock.Object,
                _jobSchedulerServiceMock.Object);

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

            var handler = new DeleteLotCommandHandler(
                _mapper,
				_lotRepository,
				_lotStatusRepository,
				_bidRepository.Object,
				_fileRepository,
				_blobServiceMock.Object,
				_blobStorageOptionsMock.Object,
                _jobSchedulerServiceMock.Object);

            var result = await handler.Handle(cmd, default);

            result.Should().BeOfType<DeletedLotResponse>();
            result.WasDeleted.Should().BeTrue();
            result.Id.Should().Be(cmd.Id);
        }
    }
}
