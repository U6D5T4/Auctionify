﻿using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Features.Rates.Commands.AddRateToSeller;
using Auctionify.Core.Entities;
using Auctionify.Infrastructure.Persistence;
using Auctionify.Infrastructure.Repositories;
using AutoMapper;
using FluentAssertions;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Auctionify.UnitTests.AddRateToSellerTests
{
	public class AddRateToSellerCommandHandlerTest
	{
		private readonly IMapper _mapper;
		private readonly IRateRepository _rateRepository;
		private readonly ILotRepository _lotRepository;
		private readonly ILotStatusRepository _lotStatusRepository;
		private readonly UserManager<User> _userManager;
		private readonly ICurrentUserService _currentUserService;
		private readonly AddRateToSellerCommandValidator _validator;

		public AddRateToSellerCommandHandlerTest() 
		{
			var mockDbContext = DbContextMock.GetMock<LotStatus, ApplicationDbContext>(
				GetLotStatuses(),
				ctx => ctx.LotStatuses
			);

			mockDbContext = DbContextMock.GetMock(
				GetLots(), 
				ctx => ctx.Lots, 
				mockDbContext
			);

			mockDbContext = DbContextMock.GetMock(
				GetRates(),
				ctx=> ctx.Rates,
				mockDbContext 
			);

			var configuration = new MapperConfiguration(
				cfg =>
					cfg.AddProfiles(
						new List<Profile>
						{
							new Application.Common.Profiles.MappingProfiles(),
							new Application.Features.Rates.Profiles.MappingProfiles(),
							new Application.Features.Users.Profiles.MappingProfiles(),
						}
					)
			);
			_mapper = new Mapper(configuration);

			_userManager = EntitiesSeeding.GetUserManagerMock();

			_currentUserService = EntitiesSeeding.GetCurrentUserServiceMock();


			_rateRepository = new RateRepository(mockDbContext.Object);
			_lotRepository = new LotRepository(mockDbContext.Object);
			_lotStatusRepository = new LotStatusRepository(mockDbContext.Object);

			_validator = new AddRateToSellerCommandValidator(
				_rateRepository,
				_lotRepository,
				_lotStatusRepository
			);
		}

		[Fact]
		public async Task Handle_WhenLotToRateSellerDoesNotExist_ReturnsValidationErrors()
		{
			// Arrange
			var command = new AddRateToSellerCommand
			{
				LotId = 100,
				Comment = "Some comment to seller",
				RatingValue = 4
			};

			// Act
			var result = await _validator.TestValidateAsync(command);

			// Assert
			result.Should().NotBeNull();
			result.IsValid.Should().BeFalse();
			result.Errors
				.Should()
				.Contain(x => x.ErrorMessage == "Lot with this Id does not exist");
		}

		[Fact]
		public async Task Handle_WhenValidRequest_ReturnsAddRateToSellerResponse()
		{
			// Arrange
			var mapperMock = new Mock<IMapper>();
			var rateRepositoryMock = new Mock<IRateRepository>();

			var handler = new AddRateToSellerCommandHandler(
				mapperMock.Object,
				rateRepositoryMock.Object,
				_lotRepository,
				_currentUserService,
				_userManager
			);

			var rate = new Rate
			{
				Id = 3,
			};

			var command = new AddRateToSellerCommand
			{
				LotId = 2,
				Comment = "Something",
				RatingValue = 4
			};

			rateRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Rate>())).ReturnsAsync(rate);
			mapperMock.Setup(m => m.Map<AddRateToSellerResponse>(rate)).Returns( new AddRateToSellerResponse
			{
				Id = rate.Id,
			});

			// Act
			var result = await handler.Handle(command, CancellationToken.None);

			// Assert
			result.Should().NotBeNull();
			result.Id.Should().Be(rate.Id);
		}

		[Fact]
		public async Task Handle_WhenLotIsNotSold_ReturnsValidationErrors()
		{
			// Arrange
			var command = new AddRateToSellerCommand { 
				LotId = 1, 
				Comment = "Some comment to seller",
				RatingValue = 4
			};

			// Act
			var result = await _validator.TestValidateAsync(command);

			// Assert
			result.Should().NotBeNull();
			result.IsValid.Should().BeFalse();
			result.Errors
				.Should()
				.Contain(x => x.ErrorMessage == "You can rate while the lot is on status active");
		}

		public static List<Rate> GetRates()
		{
			return new List<Rate>
			{
				new Rate
				{
					Id = 1,
					LotId = 2,
					SenderId = 1,
					Comment = "Something",
					ReceiverId = 2,
					RatingValue = 3
				},
				new Rate
				{
					Id = 2,
					LotId = 2,
					SenderId = 2,
					Comment = "Something",
					ReceiverId = 1,
					RatingValue = 3
				}
			};
		}

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

		public static List<Lot> GetLots()
		{
			var lotStatuses = GetLotStatuses();
			var rates = GetRates();

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
					Bids = null,
					Rates = null
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
					BuyerId = 2,
					CategoryId = 1,
					CurrencyId = 1,
					Bids = null,
					Rates = new List<Rate> { rates[0], rates[1] }
				}
			};
		}
	}
}
