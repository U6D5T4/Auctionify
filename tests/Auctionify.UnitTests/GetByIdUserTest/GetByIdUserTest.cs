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
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly GetByIdUserQueryHandler _handler;

        public GetByIdUserTests()
        {
            _userManagerMock = new Mock<UserManager<User>>();
            _handler = new GetByIdUserQueryHandler(_userManagerMock.Object, new MapperConfiguration(cfg => cfg.CreateMap<User, GetByIdUserResponse>()).CreateMapper());
        }

        [Fact]
        public async Task Handle_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var user = new User { Id = 1 };

            _userManagerMock.Setup(um => um.FindByIdAsync(user.Id.ToString())).ReturnsAsync(user);

            // Act
            var result = await _handler.Handle(new GetByIdUserQuery { Id = user.Id.ToString() }, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id.ToString(), result.Id);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenUserDoesNotExist()
        {
            // Arrange
            _userManagerMock.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(() => null);

            // Act
            var action = async () => await _handler.Handle(new GetByIdUserQuery { Id = "2" }, CancellationToken.None);

            // Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(action);
        }
    }
}
