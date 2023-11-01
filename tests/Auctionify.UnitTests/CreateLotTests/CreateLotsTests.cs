using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Features.Lots.Commands.Create;
using Auctionify.Core.Entities;
using Auctionify.Infrastructure.Persistence;
using Auctionify.Infrastructure.Repositories;
using AutoMapper;
using FluentAssertions;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Identity;
using Moq;
using System.Collections;

namespace Auctionify.UnitTests.CreateLotTests
{
    public class CreateLotsTests
    {
        private readonly IMapper _mapper;
        private readonly ILotRepository _lotRepository;
        private readonly UserManager<User> _userManager;
        private readonly ILotStatusRepository _lotStatusRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly CreateLotCommandValidator _validator;

        public CreateLotsTests() {
            var mockDbContext = DbContextMock.GetMock<Lot, ApplicationDbContext>(new List<Lot>(), ctx => ctx.Lots);
            mockDbContext = DbContextMock.GetMock(EntitiesSeeding.GetLotStatuses(), ctx => ctx.LotStatuses, mockDbContext);
            mockDbContext = DbContextMock.GetMock(EntitiesSeeding.GetCategories(), ctx => ctx.Categories, mockDbContext);
            mockDbContext = DbContextMock.GetMock(EntitiesSeeding.GetCurrencies(), ctx => ctx.Currency, mockDbContext);

            _lotRepository = new LotRepository(mockDbContext.Object);

            var configuration = new MapperConfiguration(cfg => cfg.AddProfiles(new List<Profile>
            {
                new Application.Common.Profiles.MappingProfiles(),
                new Application.Features.Lots.Profiles.MappingProfiles(),
            }));
            _mapper = new Mapper(configuration);

            var currentUserService = new Mock<ICurrentUserService>();
            currentUserService.Setup(x => x.UserEmail).Returns(It.IsAny<string>());
            _currentUserService = currentUserService.Object;

            _lotStatusRepository = new LotStatusRepository(mockDbContext.Object);

            _userManager = EntitiesSeeding.GetUserManagerMock();

            var categoryRepository = new CategoryRepository(mockDbContext.Object);
            var currencyRepository = new CurrencyRepository(mockDbContext.Object);

            _validator = new CreateLotCommandValidator(categoryRepository, _lotStatusRepository, currencyRepository);
        }

        [Fact]
        public async Task CreateLotCommandHandler_WhenCalledAsDraft_ReturnsDraftLot()
        {
            var newLot = new CreateLotCommand
            {
                Title = "Testasdasd",
                Description = "Descriptionasdasdadasdasdasdasdadaasdad",
                Address = "Test address",
                City = "Test city",
                Country = "Test country",
                IsDraft = true,
            };
            await _validator.TestValidateAsync(newLot);
            var command = new CreateLotCommandHandler(_lotRepository, _lotStatusRepository, _currentUserService, _userManager, _mapper);

            var result = await command.Handle(newLot, default);

            result.Should().BeOfType<CreatedLotResponse>();
        }

        [Fact]
        public async Task CreateLotCommandHandler_WhenDraftCalledWithWrongProperties_ShouldHaveValidationErrors()
        {
            var newLot = new CreateLotCommand
            {
                Title = "short",
                Description = "short",
                StartingPrice = 0,
                CategoryId = 4,
                CurrencyId = 4,
                IsDraft = true
            };

            var result = await _validator.TestValidateAsync(newLot, default);

            result.ShouldHaveValidationErrorFor(lot => lot.Title);
            result.ShouldHaveValidationErrorFor(lot => lot.Description);
            result.ShouldHaveValidationErrorFor(lot => lot.StartingPrice);
            result.ShouldHaveValidationErrorFor(lot => lot.Address);
            result.ShouldHaveValidationErrorFor(lot => lot.City);
            result.ShouldHaveValidationErrorFor(lot => lot.Country);
            result.ShouldHaveValidationErrorFor(lot => lot.CategoryId);
            result.ShouldHaveValidationErrorFor(lot => lot.CurrencyId);
        }

        [Fact]
        public async Task CreateLotCommandHandler_WhenActiveCalledWithWrongAndEmtpyProperties_ShouldHaveValidationErrors()
        {
            var newLot = new CreateLotCommand
            {
                Title = "TOO LONG TITLE TITLE TITLE TITLE TITLE TITLE TITLE TITLE TITLE TITEL TITLE TITTLE TITLE",
                Description = "short",
                StartingPrice = 0,
                CategoryId = 4,
                CurrencyId = 4,
                IsDraft = false
            };

            var result = await _validator.TestValidateAsync(newLot, default);

            result.ShouldHaveValidationErrorFor(lot => lot.Title);
            result.ShouldHaveValidationErrorFor(lot => lot.Description);
            result.ShouldHaveValidationErrorFor(lot => lot.StartingPrice)
                .WithErrorCode("GreaterThanValidator");
            result.ShouldHaveValidationErrorFor(lot => lot.Address)
                .WithErrorCode("NotEmptyValidator");
            result.ShouldHaveValidationErrorFor(lot => lot.City)
                .WithErrorCode("NotEmptyValidator");
            result.ShouldHaveValidationErrorFor(lot => lot.Country)
                .WithErrorCode("NotEmptyValidator");
            result.ShouldHaveValidationErrorFor(lot => lot.CategoryId);
            result.ShouldHaveValidationErrorFor(lot => lot.CurrencyId);
            result.ShouldHaveValidationErrorFor(lot => lot.StartDate)
                .WithErrorCode("NotEmptyValidator");
            result.ShouldHaveValidationErrorFor(lot => lot.Photos)
                .WithErrorCode("NotEmptyValidator");
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public async Task CreateLotCommandHandler_WhenActiveCalledWithWrongDateProperties_ShouldHaveValidationErrors
            (int testCondition)
        {
            var startDate = DateTime.MinValue.AddDays(1);
            var endDate = DateTime.MinValue;

            // Cases for dynamic DateTime.Now changes (Doesnot work with ClassData/MemberData when passed as argument to test)
            if (testCondition == 1 )
            {
                
            }
            else if (testCondition == 2)
            {
                startDate = DateTime.Now;
                endDate = DateTime.Now;
            }
            else if (testCondition == 3)
            {
                startDate = DateTime.Now.AddDays(1).AddSeconds(2);
                endDate = DateTime.Now;
            }
            else if (testCondition == 4)
            {
                startDate = DateTime.Now.AddDays(1).AddSeconds(2);
                endDate = DateTime.Now.AddHours(2);
            }

            var newLot = new CreateLotCommand
            {
                StartDate = startDate,
                EndDate = endDate,
                IsDraft = false
            };

            var result = await _validator.TestValidateAsync(newLot, default);

            if (startDate < DateTime.Now.AddDays(1))
            {
                result.ShouldHaveValidationErrorFor(lot => lot.StartDate)
                        .WithErrorCode("PredicateValidator");
            }
            result.ShouldHaveValidationErrorFor(lot => lot.EndDate)
                .WithErrorCode("PredicateValidator");
        }
    }
}
