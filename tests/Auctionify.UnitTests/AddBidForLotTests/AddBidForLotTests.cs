using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Features.Users.Commands.AddBidForLot;
using Auctionify.Core.Entities;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Auctionify.UnitTests.AddBidForLotTests
{
	public class AddBidForLotTests
	{
		[Fact]
		public async Task Handle_WhenValidRequest_ReturnsAddedBidForLotResponse()
		{
			// Arrange
			var mapperMock = new Mock<IMapper>();
			var bidRepositoryMock = new Mock<IBidRepository>();
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

			var handler = new AddBidForLotCommandHandler(
				mapperMock.Object,
				bidRepositoryMock.Object,
				currentUserServiceMock.Object,
				userManagerMock.Object
			);

			var command = new AddBidForLotCommand { LotId = 1, Bid = 100 };

			var user = new User { Id = 1, Email = "user@example.com" };
			userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);

			var addedBid = new Bid
			{
				Id = 1,
				BuyerId = user.Id,
				NewPrice = command.Bid,
				TimeStamp = DateTime.Now,
				LotId = command.LotId,
				BidRemoved = false
			};

			bidRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Bid>())).ReturnsAsync(addedBid);

			mapperMock
				.Setup(m => m.Map<AddedBidForLotResponse>(addedBid))
				.Returns(new AddedBidForLotResponse { Id = addedBid.Id });

			// Act
			var result = await handler.Handle(command, CancellationToken.None);

			// Assert
			result.Should().NotBeNull();
			result.Id.Should().Be(addedBid.Id);
			bidRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Bid>()), Times.Once);
		}
	}
}
