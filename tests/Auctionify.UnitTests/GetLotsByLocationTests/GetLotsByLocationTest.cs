using Auctionify.Application.Common.DTOs;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Models.Requests;
using Auctionify.Application.Features.Lots.Queries.GetAllByName;
using Auctionify.Core.Entities;
using Auctionify.Infrastructure.Persistence;
using Auctionify.Infrastructure.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moq;
using FluentAssertions;

namespace Auctionify.UnitTests.GetAllLotsByLocationTests
{
    public class GetAllLotsByLocationTest
    {
        private readonly IMapper _mapper;
        private readonly ILotRepository _lotRepository;
        private readonly Mock<IWatchlistService> _watchListServiceMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly Mock<IPhotoService> _photoServiceMock;
        private readonly Mock<IBidRepository> _bidRepositoryMock;
        private readonly UserManager<User> _userManager;

        public GetAllLotsByLocationTest()
        {
            var mockDbContext = DbContextMock.GetMock<Lot, ApplicationDbContext>(EntitiesSeeding.GetLots(), ctx => ctx.Lots);
            mockDbContext = DbContextMock.GetMock(EntitiesSeeding.GetFiles(), ctx => ctx.Files, mockDbContext);
            var configuration = new MapperConfiguration(cfg => cfg.AddProfiles(new List<Profile>
            {
                new Application.Common.Profiles.MappingProfiles(),
                new Application.Features.Lots.Profiles.MappingProfiles(),
            }));

            _lotRepository = new LotRepository(mockDbContext.Object);
            _watchListServiceMock = new Mock<IWatchlistService>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _photoServiceMock = new Mock<IPhotoService>();
            _userManager = EntitiesSeeding.GetUserManagerMock();
            _bidRepositoryMock = new Mock<IBidRepository>();
            _currentUserServiceMock.Setup(x => x.UserEmail).Returns(It.IsAny<string>());
            _mapper = new Mapper(configuration);
        }

        [Fact]
        public async Task GetAllLotsByLocationQueryHandler_WhenNoLotsMatchLocation_ReturnsEmptyList()
        {
            var query = new GetAllLotsByLocationQuery
            {
                PageRequest = new PageRequest
                {
                    PageIndex = 0,
                    PageSize = 10
                },
                Location = "NonExistentLocation"
            };

            var handler = new GetAllLotsByLocationQueryHandler(
                _lotRepository,
                _currentUserServiceMock.Object,
                _userManager,
                _watchListServiceMock.Object,
                _photoServiceMock.Object,
                _mapper,
                _bidRepositoryMock.Object
            );

            var result = await handler.Handle(query, default);

            result.Should().BeOfType<GetListResponseDto<GetAllLotsByLocationResponse>>();
            result.Count.Should().Be(0);
        }

        [Fact]
        public async Task GetAllLotsByLocationQueryHandler_WhenNoUser_ReturnsLotsWithoutWatchlistInfo()
        {
            _currentUserServiceMock.Setup(x => x.UserEmail).Returns((string)null);
            var query = new GetAllLotsByLocationQuery
            {
                PageRequest = new PageRequest
                {
                    PageIndex = 0,
                    PageSize = 10
                },
                Location = "London"
            };

            var handler = new GetAllLotsByLocationQueryHandler(
                _lotRepository,
                _currentUserServiceMock.Object,
                _userManager,
                _watchListServiceMock.Object,
                _photoServiceMock.Object,
                _mapper,
                _bidRepositoryMock.Object
            );

            var result = await handler.Handle(query, default);

            result.Should().BeOfType<GetListResponseDto<GetAllLotsByLocationResponse>>();
            result.Items.All(lot => lot.IsInWatchlist == false).Should().BeTrue();
        }

        [Fact]
        public async Task GetAllLotsByLocationQueryHandler_WhenPageSizeIsZero_ReturnsEmptyList()
        {
            // Arrange
            var query = new GetAllLotsByLocationQuery
            {
                PageRequest = new PageRequest
                {
                    PageIndex = 0,
                    PageSize = 0
                },
                Location = "London"
            };

            var handler = new GetAllLotsByLocationQueryHandler(
                _lotRepository,
                _currentUserServiceMock.Object,
                _userManager,
                _watchListServiceMock.Object,
                _photoServiceMock.Object,
                _mapper,
                _bidRepositoryMock.Object
            );

            // Act
            var result = await handler.Handle(query, default);

            // Assert
            result.Should().BeOfType<GetListResponseDto<GetAllLotsByLocationResponse>>();
            result.Count.Should().Be(0);
        }

        [Fact]
        public async Task GetAllLotsByLocationQueryHandler_WhenInvalidLocation_ReturnsEmptyList()
        {
            var query = new GetAllLotsByLocationQuery
            {
                PageRequest = new PageRequest
                {
                    PageIndex = 0,
                    PageSize = 10
                },
                Location = "InvalidLocation"
            };

            var handler = new GetAllLotsByLocationQueryHandler(
                _lotRepository,
                _currentUserServiceMock.Object,
                _userManager,
                _watchListServiceMock.Object,
                _photoServiceMock.Object,
                _mapper,
                _bidRepositoryMock.Object
            );

            var result = await handler.Handle(query, default);

            result.Should().BeOfType<GetListResponseDto<GetAllLotsByLocationResponse>>();
            result.Count.Should().Be(0);
        }
    }
}