using Auctionify.Application.Features.Users.Queries.GetById;
using Auctionify.Core.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace Auctionify.UnitTests.GetByIdUserTest
{
    public class GetByIdUserTests
    {
        [Fact]
        public async Task Handle_ValidId_ReturnsUser()
        {
            var userId = 1;
            var user = new User { Id = userId, FirstName = "JohnDoe" };

            var userManagerMock = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            userManagerMock.Setup(m => m.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map<GetByIdUserResponse>(user))
                .Returns(new GetByIdUserResponse { Id = userId.ToString(), FirstName = "JohnDoe" });

            var handler = new GetByIdUserQueryHandler(userManagerMock.Object, mapperMock.Object);
            var query = new GetByIdUserQuery { Id = userId.ToString() };

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(userId.ToString(), result.Id);
            Assert.Equal("JohnDoe", result.FirstName);
        }

        [Fact]
        public async Task Handle_InvalidId_ReturnsNull()
        {
            var userId = 2;

            var userManagerMock = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            userManagerMock.Setup(m => m.FindByIdAsync(userId.ToString()))
                .ReturnsAsync((User)null);

            var mapperMock = new Mock<IMapper>();

            var handler = new GetByIdUserQueryHandler(userManagerMock.Object, mapperMock.Object);
            var query = new GetByIdUserQuery { Id = userId.ToString() };

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.Null(result);
        }
    }
}
