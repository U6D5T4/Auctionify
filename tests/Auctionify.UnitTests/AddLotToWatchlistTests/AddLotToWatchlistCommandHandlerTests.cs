using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Features.Users.Commands.AddLotToWatchlist;
using Auctionify.Core.Entities;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Auctionify.UnitTests.AddLotToWatchlistTests
{
	public class AddLotToWatchlistCommandHandlerTests
	{
		#region Tests

		[Fact]
		public async Task Handle_WhenValidRequest_ReturnsAddedToWatchlistResponse()
		{
			// Arrange
			var mapperMock = new Mock<IMapper>();
			var watchlistRepositoryMock = new Mock<IWatchlistRepository>();
			var currentUserServiceMock = new Mock<ICurrentUserService>();
			var userManagerMock = new Mock<UserManager<User>>(
				Mock.Of<IUserStore<User>>(),
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
			);

			var handler = new AddToWatchListCommandHandler(
				mapperMock.Object,
				watchlistRepositoryMock.Object,
				currentUserServiceMock.Object,
				userManagerMock.Object
			);

			var command = new AddToWatchlistCommand
			{
				LotId = 1
			};

			var user = new User { Id = 1, Email = "user@example.com" };
			userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);

			var watchlist = new Watchlist { UserId = user.Id, LotId = command.LotId };

			watchlistRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Watchlist>())).ReturnsAsync(watchlist);

			mapperMock.Setup(m => m.Map<AddedToWatchlistResponse>(watchlist))
				.Returns(new AddedToWatchlistResponse { Id = 1 });

			// Act
			var result = await handler.Handle(command, CancellationToken.None);

			// Assert
			result.Should().NotBeNull();
			result.Id.Should().Be(1);
			userManagerMock.Verify(x => x.FindByEmailAsync(It.IsAny<string>()), Times.Once);
			watchlistRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Watchlist>()), Times.Once);
			mapperMock.Verify(m => m.Map<AddedToWatchlistResponse>(watchlist), Times.Once);
		}

		#endregion
	}
}
