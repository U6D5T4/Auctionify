using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Common.Options;
using Auctionify.Application.Features.Lots.Queries.GetByIdForBuyer;
using Auctionify.Application.Features.Lots.Queries.GetByIdForSeller;
using Auctionify.Core.Entities;
using Auctionify.Infrastructure.Persistence;
using Auctionify.Infrastructure.Repositories;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;

namespace Auctionify.UnitTests.GetLotByIdTests
{
    public class GetLotByIdTests
    {
        private readonly IMapper _mapper;
        private readonly ILotRepository _lotRepository;
		private readonly IFileRepository _fileRepository;
        private readonly Mock<IWatchlistService> _watchListServiceMock;
		private readonly Mock<IBlobService> _blobServiceMock;
        private readonly Mock<IOptions<AzureBlobStorageOptions>> _blobStorageOptionsMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly UserManager<User> _userManager;

        public GetLotByIdTests()
        {
            var mockDbContext = DbContextMock.GetMock<Lot, ApplicationDbContext>(EntitiesSeeding.GetLots(), ctx => ctx.Lots);
            mockDbContext = DbContextMock.GetMock(EntitiesSeeding.GetFiles(), ctx => ctx.Files, mockDbContext);
            var blobStorageOptionsMock = new Mock<IOptions<AzureBlobStorageOptions>>();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfiles(new List<Profile>
            {
                new Application.Common.Profiles.MappingProfiles(),
                new Application.Features.Lots.Profiles.MappingProfiles(),
            }));

            _lotRepository = new LotRepository(mockDbContext.Object);
            _fileRepository = new FileRepository(mockDbContext.Object);
            _watchListServiceMock = new Mock<IWatchlistService>();
            _blobServiceMock = new Mock<IBlobService>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _userManager = EntitiesSeeding.GetUserManagerMock();

            _currentUserServiceMock.Setup(x => x.UserEmail).Returns(It.IsAny<string>());
			blobStorageOptionsMock.Setup(x => x.Value).Returns(new AzureBlobStorageOptions
			{
				ContainerName = "auctionify-files",
				PhotosFolderName = "photos",
				AdditionalDocumentsFolderName = "additional-documents"
			});

            _blobStorageOptionsMock = blobStorageOptionsMock;
			_mapper = new Mapper(configuration);
        }

        [Fact]
        public async Task GetByIdLotForBuyerQueryHandler_WhenCalledWithCorrectId_ReturnsLot()
        {
            //Lot is not in buyer's watchlist
            _watchListServiceMock.Setup(x => x.IsLotInUserWatchlist(It.IsAny<int>(), It.IsAny<int>(), default)).ReturnsAsync(false);

            // Lot without files
            var query = new GetByIdForBuyerLotQuery
            {
                Id = 2,
            };

            var handler = new GetByIdForBuyerLotQueryHandler(
                _lotRepository,
                _mapper,
                _blobServiceMock.Object,
                _fileRepository,
                _blobStorageOptionsMock.Object,
                _currentUserServiceMock.Object,
                _userManager,
                _watchListServiceMock.Object);

            var result = await handler.Handle(query, default);

            result.Should().BeOfType<GetByIdForBuyerLotResponse>();
            result.Id.Should().Be(query.Id);
        }

		[Fact]
		public async Task GetByIdLotForBuyerQueryHandler_WhenCalledWithCorrectIdAndIsInBuyerWatchlist_ReturnsLotWithWatchlistTrue()
		{
			//Lot is not in buyer's watchlist
			_watchListServiceMock.Setup(x => x.IsLotInUserWatchlist(It.IsAny<int>(), It.IsAny<int>(), default)).ReturnsAsync(true);

			// Lot without files
			var query = new GetByIdForBuyerLotQuery
			{
				Id = 2,
			};

			var handler = new GetByIdForBuyerLotQueryHandler(
				_lotRepository,
				_mapper,
				_blobServiceMock.Object,
				_fileRepository,
				_blobStorageOptionsMock.Object,
				_currentUserServiceMock.Object,
				_userManager,
				_watchListServiceMock.Object);

			var result = await handler.Handle(query, default);

			result.Should().BeOfType<GetByIdForBuyerLotResponse>();
			result.Id.Should().Be(query.Id);
            result.IsInWatchlist.Should().BeTrue();
		}

		[Fact]
		public async Task GetByIdLotForBuyerQueryHandler_WhenCalledWithCorrectIdAndHasFiles_ReturnsLotWithFiles()
		{
			//Lot is not in buyer's watchlist
			_watchListServiceMock.Setup(x => x.IsLotInUserWatchlist(It.IsAny<int>(), It.IsAny<int>(), default)).ReturnsAsync(true);

			// Lot with files
			var query = new GetByIdForBuyerLotQuery
			{
				Id = 1,
			};

			var handler = new GetByIdForBuyerLotQueryHandler(
				_lotRepository,
				_mapper,
				_blobServiceMock.Object,
				_fileRepository,
				_blobStorageOptionsMock.Object,
				_currentUserServiceMock.Object,
				_userManager,
				_watchListServiceMock.Object);

			var result = await handler.Handle(query, default);

			result.Should().BeOfType<GetByIdForBuyerLotResponse>();
			result.Id.Should().Be(query.Id);
			result.PhotosUrl.Count.Should().Be(1);
			result.AdditionalDocumentsUrl.Count.Should().Be(1);
		}

		[Fact]
		public async Task GetByIdLotForBuyerQueryHandler_WhenCalledWithWrongId_ReturnsNull()
		{
			//Lot is not in buyer's watchlist
			_watchListServiceMock.Setup(x => x.IsLotInUserWatchlist(It.IsAny<int>(), It.IsAny<int>(), default)).ReturnsAsync(true);

			// Lot with files
			var query = new GetByIdForBuyerLotQuery
			{
				Id = 0,
			};

			var handler = new GetByIdForBuyerLotQueryHandler(
				_lotRepository,
				_mapper,
				_blobServiceMock.Object,
				_fileRepository,
				_blobStorageOptionsMock.Object,
				_currentUserServiceMock.Object,
				_userManager,
				_watchListServiceMock.Object);

			var result = await handler.Handle(query, default);

			result.Should().BeNull();
		}

		[Fact]
		public async Task GetByIdLotForSellerQueryHandler_WhenCalledWithCorrectIdAndCreatedBySeller_ReturnsLot()
		{
			// Lot created by user with Id 1
			var query = new GetByIdForSellerLotQuery
			{
				Id = 1,
			};

			var handler = new GetByIdForSellerLotQueryHandler(
				_lotRepository,
				_mapper,
				_blobServiceMock.Object,
				_fileRepository,
				_blobStorageOptionsMock.Object,
				_currentUserServiceMock.Object,
				_userManager);

			var result = await handler.Handle(query, default);

			result.Id.Should().Be(1);
		}

		[Fact]
		public async Task GetByIdLotForSellerQueryHandler_WhenCalledWithCorrectIdAndNotCreatedBySeller_ReturnsNull()
		{
			// Lot created by other user id
			var query = new GetByIdForSellerLotQuery
			{
				Id = 4,
			};

			var handler = new GetByIdForSellerLotQueryHandler(
				_lotRepository,
				_mapper,
				_blobServiceMock.Object,
				_fileRepository,
				_blobStorageOptionsMock.Object,
				_currentUserServiceMock.Object,
				_userManager);

			var result = await handler.Handle(query, default);

			result.Should().BeNull();
		}
	}
}
