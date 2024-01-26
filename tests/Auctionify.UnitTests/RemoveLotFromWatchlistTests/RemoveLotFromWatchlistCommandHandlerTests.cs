using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Features.Users.Commands.RemoveLotFromWatchlist;
using Auctionify.Core.Entities;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using System.Linq.Expressions;

namespace Auctionify.UnitTests.RemoveLotFromWatchlistTests
{
	public class RemoveLotFromWatchlistCommandHandlerTests
	{
		#region Initialization

		private readonly ICurrentUserService _currentUserService;
		private readonly UserManager<User> _userManager;

		public RemoveLotFromWatchlistCommandHandlerTests()
		{
			_userManager = EntitiesSeeding.GetUserManagerMock();
			_currentUserService = EntitiesSeeding.GetCurrentUserServiceMock();
		}

		#endregion

		[Fact]
		public async Task Handle_WhenValidRequest_ReturnsRemovedLotFromWatchlistResponse()
		{
			// Arrange
			var mapperMock = new Mock<IMapper>();
			var watchlistRepositoryMock = new Mock<IWatchlistRepository>();

			var handler = new RemoveLotFromWatchlistCommandHandler(
				mapperMock.Object,
				watchlistRepositoryMock.Object,
				_currentUserService,
				_userManager
			);

			var command = new RemoveLotFromWatchlistCommand { LotId = 1 };

			var user = new User { Id = 1, Email = "user@example.com" };

			var watchlist = new Watchlist { UserId = user.Id, LotId = command.LotId };

			watchlistRepositoryMock
				.Setup(
					x =>
						x.GetAsync(
							It.IsAny<Expression<Func<Watchlist, bool>>>(),
							null,
							false,
							true,
							default
						)
				)
				.ReturnsAsync(watchlist);

			watchlistRepositoryMock
				.Setup(x => x.DeleteAsync(watchlist, false))
				.ReturnsAsync(watchlist);

			mapperMock
				.Setup(m => m.Map<RemovedLotFromWatchlistResponse>(watchlist))
				.Returns(new RemovedLotFromWatchlistResponse { Id = 1 });

			// Act
			var result = await handler.Handle(command, CancellationToken.None);

			// Assert
			result.Should().NotBeNull();
			result.Id.Should().Be(1);
			watchlistRepositoryMock.Verify(
				x =>
					x.GetAsync(
						It.IsAny<Expression<Func<Watchlist, bool>>>(),
						null,
						false,
						true,
						default
					),
				Times.Once
			);
			watchlistRepositoryMock.Verify(x => x.DeleteAsync(watchlist, false), Times.Once);
			mapperMock.Verify(m => m.Map<RemovedLotFromWatchlistResponse>(watchlist), Times.Once);
		}
	}
}
