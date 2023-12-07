using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Options;
using AutoMapper;
using Microsoft.Extensions.Options;
using Moq;
using Auctionify.Core.Entities;
using Auctionify.Infrastructure.Persistence;
using Auctionify.Infrastructure.Repositories;
using Auctionify.Application.Features.Lots.Commands.Update;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using FluentValidation;

namespace Auctionify.UnitTests.UpdateLotTests
{
    public class UpdateLotTest
    {
        private readonly ILotRepository _lotRepository;
        private readonly IMapper _mapper;
        private readonly ILotStatusRepository _lotStatusRepository;
        private readonly IFileRepository _fileRepository;
        private readonly Mock<IBidRepository> _bidRepository;
        private readonly Mock<IBlobService> _blobServiceMock;
        private readonly Mock<IOptions<AzureBlobStorageOptions>> _blobStorageOptionsMock;

        public UpdateLotTest()
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
            bidRepository.CallBase = true;
            _blobServiceMock = new Mock<IBlobService>();

            _bidRepository = bidRepository;
        }

        [Fact]
        public async Task Handle_ValidUpdate_ShouldUpdateLot()
        {
            var updateCommand = new UpdateLotCommand
            {
                Id = 1,
                Title = "Updated Lot Title",
                Description = "Updated Lot Description",
                StartingPrice = 150,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(7),
                CategoryId = 2,
                City = "Updated City",
                Country = "Updated Country",
                Address = "Updated Address",
                CurrencyId = 2,
                IsDraft = false,
            };

            var handler = new UpdateLotCommandHandler(
                _lotRepository,
                _lotStatusRepository,
                _mapper,
                _blobServiceMock.Object, 
                _fileRepository,
                _blobStorageOptionsMock.Object
            );

            var result = await handler.Handle(updateCommand, default);

            result.Should().NotBeNull();
            result.Id.Should().Be(updateCommand.Id);
            result.Title.Should().Be(updateCommand.Title);

            var updatedLot = await _lotRepository.GetAsync(lot => lot.Id == updateCommand.Id);
            updatedLot.Should().NotBeNull();
            updatedLot.Title.Should().Be(updateCommand.Title);
        }

        [Fact]
        public async Task Handle_UpdateDraftLot_ShouldNotUpdatePhotosAndDocuments()
        {
            var existingLotId = 1;
            var existingLot = EntitiesSeeding.GetLots().Find(l => l.Id == existingLotId);

            var updateCommand = new UpdateLotCommand
            {
                Id = existingLotId,
                Title = "Updated Draft Lot",
                Description = "Updated description",
                StartingPrice = 100.0m,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(7),
                CategoryId = 1,
                City = "New City",
                State = "New State",
                Country = "New Country",
                Address = "New Address",
                CurrencyId = 1,
                Photos = null,
                AdditionalDocuments = null,
                IsDraft = true,
            };

            var handler = new UpdateLotCommandHandler(
                _lotRepository,
                _lotStatusRepository,
                _mapper,
                _blobServiceMock.Object,
                _fileRepository,
                _blobStorageOptionsMock.Object
            );

            var result = await handler.Handle(updateCommand, default);

            result.Should().NotBeNull();
            result.Id.Should().Be(existingLotId);
            result.Title.Should().Be(updateCommand.Title);

            var updatedLot = await _lotRepository.GetAsync(lot => lot.Id == existingLotId);
            updatedLot.Should().NotBeNull();
            updatedLot.Title.Should().Be(updateCommand.Title);
            updatedLot.Description.Should().Be(updateCommand.Description);
            updatedLot.StartingPrice.Should().Be(updateCommand.StartingPrice);
        }

        [Fact]
        public async Task Handle_UpdateNonDraftLot_ShouldUpdatePhotosAndDocuments()
        {
            var existingLotId = 1;
            var existingLot = EntitiesSeeding.GetLots().Find(l => l.Id == existingLotId);

            var updateCommand = new UpdateLotCommand
            {
                Id = existingLotId,
                Title = "Updated Lot",
                Description = "Updated description",
                StartingPrice = 100.0m,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(7),
                CategoryId = 1,
                City = "New City",
                State = "New State",
                Country = "New Country",
                Address = "New Address",
                CurrencyId = 1,
                Photos = new List<IFormFile> { },
                AdditionalDocuments = new List<IFormFile> { },
                IsDraft = false,
            };

            var handler = new UpdateLotCommandHandler(
                _lotRepository,
                _lotStatusRepository,
                _mapper,
                _blobServiceMock.Object,
                _fileRepository,
                _blobStorageOptionsMock.Object
            );

            var result = await handler.Handle(updateCommand, default);

            result.Should().NotBeNull();
            result.Id.Should().Be(existingLotId);
            result.Title.Should().Be(updateCommand.Title);

            var updatedLot = await _lotRepository.GetAsync(lot => lot.Id == existingLotId);
            updatedLot = await _lotRepository.GetAsync(lot => lot.Id == existingLotId);
            updatedLot.Should().NotBeNull();
            updatedLot.Title.Should().Be(updateCommand.Title);
            updatedLot.Description.Should().Be(updateCommand.Description);
            updatedLot.StartingPrice.Should().Be(updateCommand.StartingPrice);
        }
    }
}
