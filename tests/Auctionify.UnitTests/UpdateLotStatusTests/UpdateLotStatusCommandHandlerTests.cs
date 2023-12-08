using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Features.Lots.Commands.UpdateLotStatus;
using Auctionify.Core.Entities;
using Auctionify.Infrastructure.Persistence;
using Auctionify.Infrastructure.Repositories;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.TestHelper;

namespace Auctionify.UnitTests.UpdateLotStatusTests
{
	public class UpdateLotStatusCommandHandlerTests
	{
		#region Initialization

		private readonly IMapper _mapper;
		private readonly ILotRepository _lotRepository;
		private readonly ILotStatusRepository _lotStatusRepository;
		private readonly UpdateLotStatusCommandValidator _validator;

		public UpdateLotStatusCommandHandlerTests()
		{
			var mockDbContext = DbContextMock.GetMock<Lot, ApplicationDbContext>(
				GetLots(),
				ctx => ctx.Lots
			);
			mockDbContext = DbContextMock.GetMock(
				GetLotStatuses(),
				ctx => ctx.LotStatuses,
				mockDbContext
			);
			mockDbContext = DbContextMock.GetMock(GetBids(), ctx => ctx.Bids, mockDbContext);

			var configuration = new MapperConfiguration(
				cfg =>
					cfg.AddProfiles(
						new List<Profile>
						{
							new Application.Common.Profiles.MappingProfiles(),
							new Application.Features.Lots.Profiles.MappingProfiles(),
						}
					)
			);
			_mapper = new Mapper(configuration);

			_lotRepository = new LotRepository(mockDbContext.Object);
			_lotStatusRepository = new LotStatusRepository(mockDbContext.Object);
			_validator = new UpdateLotStatusCommandValidator(_lotStatusRepository);
		}

		#endregion

		#region Tests

		[Fact]
		public async Task Handle_GivenValidRequest_ShouldReturnUpdatedLotStatusResponse()
		{
			var handler = new UpdateLotStatusCommandHandler(
				_lotRepository,
				_lotStatusRepository,
				_mapper
			);

			var request = new UpdateLotStatusCommand { LotId = 1, Name = "Sold" };

			var result = await handler.Handle(request, CancellationToken.None);

			result.Should().BeOfType<UpdatedLotStatusResponse>();
			result.Should().NotBeNull();
			result
				.Should()
				.BeEquivalentTo(new UpdatedLotStatusResponse { LotId = 1, Name = "Sold" });
		}

		[Fact]
		public async Task Handle_GivenInvalidRequest_ShouldThrowValidationException()
		{
			var handler = new UpdateLotStatusCommandHandler(
				_lotRepository,
				_lotStatusRepository,
				_mapper
			);

			var request = new UpdateLotStatusCommand { LotId = 1, Name = "Archive", };

			await Assert.ThrowsAsync<ValidationException>(
				async () => await handler.Handle(request, CancellationToken.None)
			);
		}

		[Fact]
		public async Task Handle_GivenIncorrectLotStatusName_ShouldThrowValidationException()
		{
			var command = new UpdateLotStatusCommand { LotId = 1, Name = "IncorrectLotStatus" };

			var result = await _validator.TestValidateAsync(command);

			result.Should().NotBeNull();
			result.IsValid.Should().BeFalse();
			result.Errors
				.Should()
				.Contain(x => x.ErrorMessage == "Lot status with this name does not exist");
		}

		#endregion

		#region Helpers

		public static List<LotStatus> GetLotStatuses()
		{
			return new List<LotStatus>
			{
				new LotStatus { Id = 1, Name = "Draft" },
				new LotStatus { Id = 2, Name = "PendingApproval" },
				new LotStatus { Id = 3, Name = "Rejected" },
				new LotStatus { Id = 4, Name = "Upcoming" },
				new LotStatus { Id = 5, Name = "Active" },
				new LotStatus { Id = 6, Name = "Sold" },
				new LotStatus { Id = 7, Name = "NotSold" },
				new LotStatus { Id = 8, Name = "Cancelled" },
				new LotStatus { Id = 9, Name = "Reopened" },
				new LotStatus { Id = 10, Name = "Archive" }
			};
		}

		public static List<Bid> GetBids()
		{
			return new List<Bid>
			{
				new Bid
				{
					Id = 1,
					LotId = 1,
					BuyerId = 2,
					NewPrice = 120,
					BidRemoved = false,
				},
				new Bid
				{
					Id = 2,
					LotId = 1,
					BuyerId = 3,
					NewPrice = 140,
					BidRemoved = false,
				},
				new Bid
				{
					Id = 3,
					LotId = 2,
					BuyerId = 3,
					NewPrice = 160,
					BidRemoved = false,
				}
			};
		}

		public static List<Lot> GetLots()
		{
			var bids = GetBids();
			var lotStatuses = GetLotStatuses();

			return new List<Lot>
			{
				new Lot
				{
					Id = 1,
					Title = "Test Lot Title",
					Description =
						"Test Lot Description - Test Lot Description - Test Lot Description",
					LotStatusId = 5,
					LotStatus = lotStatuses.Find(x => x.Id == 5)!,
					StartDate = DateTime.Now,
					EndDate = DateTime.Now.AddDays(1),
					Location = new Location
					{
						Address = "Test Address",
						City = "Test City",
						Country = "Test Country",
					},
					StartingPrice = 100,
					SellerId = 1,
					CategoryId = 1,
					CurrencyId = 1,
					Bids = new List<Bid> { bids[0], bids[1], }
				},
				new Lot
				{
					Id = 2,
					Title = "Test Lot Title 2",
					Description =
						"Test Lot Description - Test Lot Description - Test Lot Description",
					LotStatusId = 6,
					LotStatus = lotStatuses.Find(x => x.Id == 6)!,
					StartDate = DateTime.Now,
					EndDate = DateTime.Now.AddDays(1),
					Location = new Location
					{
						Address = "Test Address",
						City = "Test City",
						Country = "Test Country",
					},
					StartingPrice = 100,
					SellerId = 1,
					CategoryId = 1,
					CurrencyId = 1,
					Bids = new List<Bid> { bids[2], }
				}
			};
		}

		#endregion
	}
}
