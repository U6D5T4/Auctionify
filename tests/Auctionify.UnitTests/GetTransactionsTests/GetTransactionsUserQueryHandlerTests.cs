using Auctionify.Application.Common.DTOs;
using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Common.Models.Requests;
using Auctionify.Application.Common.Models.Transaction;
using Auctionify.Application.Features.Users.Queries.GetTransactions;
using Auctionify.Core.Entities;
using Auctionify.Core.Enums;
using Auctionify.Core.Persistence.Paging;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Query;
using MockQueryable.Moq;
using Moq;
using System.Linq.Expressions;

namespace Auctionify.UnitTests.GetTransactionsTests
{
	public class GetTransactionsUserQueryHandlerTests : IDisposable
	{
		#region Initialization

		private readonly Mock<ILotRepository> _lotRepositoryMock;
		private readonly Mock<IBidRepository> _bidRepositoryMock;
		private readonly Mock<IMapper> _mapperMock;
		private readonly Mock<IPhotoService> _photoServiceMock;
		private readonly Mock<ICurrentUserService> _currentUserServiceMock;
		private readonly Mock<UserManager<User>> _userManagerMock;

		public GetTransactionsUserQueryHandlerTests()
		{
			_lotRepositoryMock = new Mock<ILotRepository>();
			_bidRepositoryMock = new Mock<IBidRepository>();
			_mapperMock = new Mock<IMapper>();
			_photoServiceMock = new Mock<IPhotoService>();
			_currentUserServiceMock = new Mock<ICurrentUserService>();
			_userManagerMock = new Mock<UserManager<User>>(
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

			_currentUserServiceMock.Setup(x => x.UserEmail).Returns(It.IsAny<string>());

			_mapperMock
				.Setup(
					m =>
						m.Map<GetListResponseDto<GetTransactionsUserResponse>>(
							It.IsAny<IPaginate<TransactionInfo>>()
						)
				)
				.Returns(
					(IPaginate<TransactionInfo> source) =>
					{
						var responseList = source
							.Items.Select(
								transactionInfo =>
									new GetTransactionsUserResponse
									{
										LotId = transactionInfo.LotId,
										LotTitle = transactionInfo.LotTitle,
										LotMainPhotoUrl = transactionInfo.LotMainPhotoUrl,
										TransactionDate = transactionInfo.TransactionDate,
										TransactionStatus = transactionInfo.TransactionStatus,
										TransactionAmount = transactionInfo.TransactionAmount,
										TransactionCurrency = transactionInfo.TransactionCurrency
									}
							)
							.ToList();

						return new GetListResponseDto<GetTransactionsUserResponse>
						{
							Items = responseList,
							Index = source.Index,
							Size = source.Size,
							Count = source.Count,
							Pages = source.Pages,
							HasPrevious = source.HasPrevious,
							HasNext = source.HasNext
						};
					}
				);
		}

		#endregion

		#region Tests

		[Theory]
		[MemberData(
			nameof(TransactionTestData.GetTestData),
			MemberType = typeof(TransactionTestData)
		)]
		public async Task Handle_ReturnsCorrectResponse_ForBuyerRole(
			List<Lot> lots,
			Bid highestBid,
			PageRequest pageRequest
		)
		{
			// Arrange
			var query = new GetTransactionsUserQuery { PageRequest = pageRequest };

			var user = new User { Id = 1, Email = "buyer@example.com" };

			var roles = new List<string> { UserRole.Buyer.ToString() };

			var mock = new List<User> { user }
				.AsQueryable()
				.BuildMockDbSet();

			_userManagerMock.Setup(m => m.Users).Returns(mock.Object);

			_currentUserServiceMock.Setup(m => m.UserEmail).Returns(user.Email);

			_userManagerMock.Setup(m => m.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(roles);

			_lotRepositoryMock
				.Setup(
					m =>
						m.GetUnpaginatedListAsync(
							It.IsAny<Expression<Func<Lot, bool>>>(),
							It.IsAny<Func<IQueryable<Lot>, IOrderedQueryable<Lot>>>(),
							It.IsAny<Func<IQueryable<Lot>, IIncludableQueryable<Lot, object>>>(),
							It.IsAny<bool>(),
							It.IsAny<bool>(),
							It.IsAny<CancellationToken>()
						)
				)
				.ReturnsAsync(lots);

			_bidRepositoryMock
				.Setup(
					m =>
						m.GetUnpaginatedListAsync(
							It.IsAny<Expression<Func<Bid, bool>>>(),
							It.IsAny<Func<IQueryable<Bid>, IOrderedQueryable<Bid>>>(),
							It.IsAny<Func<IQueryable<Bid>, IIncludableQueryable<Bid, object>>>(),
							It.IsAny<bool>(),
							It.IsAny<bool>(),
							It.IsAny<CancellationToken>()
						)
				)
				.ReturnsAsync(new List<Bid> { highestBid });

			var queryHandler = new GetTransactionsUserQueryHandler(
				_lotRepositoryMock.Object,
				_bidRepositoryMock.Object,
				_mapperMock.Object,
				_photoServiceMock.Object,
				_currentUserServiceMock.Object,
				_userManagerMock.Object
			);

			// Act
			var result = await queryHandler.Handle(query, CancellationToken.None);

			// Assert
			result.Should().NotBeNull();
			result.Items.Should().NotBeEmpty();
			result.Items.Should().HaveCount(1);
		}

		[Theory]
		[MemberData(
			nameof(TransactionTestData.GetTestData),
			MemberType = typeof(TransactionTestData)
		)]
		public async Task Handle_ReturnsCorrectResponse_ForSellerRole(
			List<Lot> lots,
			Bid highestBid,
			PageRequest pageRequest
		)
		{
			// Arrange
			var query = new GetTransactionsUserQuery { PageRequest = pageRequest };

			var user = new User { Id = 1, Email = "seller@example.com" };

			var roles = new List<string> { UserRole.Seller.ToString() };

			var mock = new List<User> { user }
				.AsQueryable()
				.BuildMockDbSet();

			_userManagerMock.Setup(m => m.Users).Returns(mock.Object);

			_currentUserServiceMock.Setup(m => m.UserEmail).Returns(user.Email);

			_userManagerMock.Setup(m => m.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(roles);

			_lotRepositoryMock
				.Setup(
					m =>
						m.GetUnpaginatedListAsync(
							It.IsAny<Expression<Func<Lot, bool>>>(),
							It.IsAny<Func<IQueryable<Lot>, IOrderedQueryable<Lot>>>(),
							It.IsAny<Func<IQueryable<Lot>, IIncludableQueryable<Lot, object>>>(),
							It.IsAny<bool>(),
							It.IsAny<bool>(),
							It.IsAny<CancellationToken>()
						)
				)
				.ReturnsAsync(lots);

			_bidRepositoryMock
				.Setup(
					m =>
						m.GetUnpaginatedListAsync(
							It.IsAny<Expression<Func<Bid, bool>>>(),
							It.IsAny<Func<IQueryable<Bid>, IOrderedQueryable<Bid>>>(),
							It.IsAny<Func<IQueryable<Bid>, IIncludableQueryable<Bid, object>>>(),
							It.IsAny<bool>(),
							It.IsAny<bool>(),
							It.IsAny<CancellationToken>()
						)
				)
				.ReturnsAsync(new List<Bid> { highestBid });

			var queryHandler = new GetTransactionsUserQueryHandler(
				_lotRepositoryMock.Object,
				_bidRepositoryMock.Object,
				_mapperMock.Object,
				_photoServiceMock.Object,
				_currentUserServiceMock.Object,
				_userManagerMock.Object
			);

			// Act
			var result = await queryHandler.Handle(query, CancellationToken.None);

			// Assert
			result.Should().NotBeNull();
			result.Items.Should().NotBeEmpty();
			result.Items.Should().HaveCount(3);
		}

		#endregion

		#region Deinitialization

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				_lotRepositoryMock.Reset();
				_bidRepositoryMock.Reset();
				_mapperMock.Reset();
				_photoServiceMock.Reset();
				_currentUserServiceMock.Reset();
				_userManagerMock.Reset();
			}
		}

		#endregion
	}
}
