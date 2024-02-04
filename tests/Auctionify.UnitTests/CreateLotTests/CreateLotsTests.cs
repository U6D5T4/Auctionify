using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Common.Options;
using Auctionify.Application.Features.Lots.Commands.Create;
using Auctionify.Core.Entities;
using Auctionify.Infrastructure.Persistence;
using Auctionify.Infrastructure.Repositories;
using AutoMapper;
using FluentAssertions;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;

namespace Auctionify.UnitTests.CreateLotTests
{
	public class CreateLotsTests
	{
		private readonly IMapper _mapper;
		private readonly ILotRepository _lotRepository;
		private readonly IFileRepository _fileRepository;
		private readonly ILotStatusRepository _lotStatusRepository;
		private readonly Mock<IBlobService> _blobServiceMock;
		private readonly Mock<IOptions<AzureBlobStorageOptions>> _blobStorageOptionsMock;
		private readonly Mock<IJobSchedulerService> _jobSchedulerServiceMock;
		private readonly UserManager<User> _userManager;
		private readonly ICurrentUserService _currentUserService;
		private readonly CreateLotCommandValidator _validator;
		private static readonly DateTime CustomDateTimeNow = new DateTime(2023, 11, 2, 13, 0, 0);


		public CreateLotsTests()
		{
			var mockDbContext = DbContextMock.GetMock<Lot, ApplicationDbContext>(new List<Lot>(), ctx => ctx.Lots);
			mockDbContext = DbContextMock.GetMock(EntitiesSeeding.GetLotStatuses(), ctx => ctx.LotStatuses, mockDbContext);
			mockDbContext = DbContextMock.GetMock(EntitiesSeeding.GetCategories(), ctx => ctx.Categories, mockDbContext);
			mockDbContext = DbContextMock.GetMock(EntitiesSeeding.GetCurrencies(), ctx => ctx.Currency, mockDbContext);

			var blobStorageOptionsMock = new Mock<IOptions<AzureBlobStorageOptions>>();
			var configuration = new MapperConfiguration(cfg => cfg.AddProfiles(new List<Profile>
			{
				new Application.Common.Profiles.MappingProfiles(),
				new Application.Features.Lots.Profiles.MappingProfiles(),
			}));
			_mapper = new Mapper(configuration);

			var jobSchedulerService = new Mock<IJobSchedulerService>();
			_blobServiceMock = new Mock<IBlobService> { CallBase = true };
			_jobSchedulerServiceMock = jobSchedulerService;
			_userManager = EntitiesSeeding.GetUserManagerMock();
			_currentUserService = EntitiesSeeding.GetCurrentUserServiceMock();

			blobStorageOptionsMock.Setup(x => x.Value).Returns(new AzureBlobStorageOptions
			{
				ContainerName = "auctionify-files",
				PhotosFolderName = "photos",
				AdditionalDocumentsFolderName = "additional-documents"
			});

			_blobStorageOptionsMock = blobStorageOptionsMock;
			_lotRepository = new LotRepository(mockDbContext.Object);
			_lotStatusRepository = new LotStatusRepository(mockDbContext.Object);
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
			var command = new CreateLotCommandHandler(
				_lotRepository,
				_lotStatusRepository,
				_currentUserService,
				_userManager,
				_mapper,
				_blobServiceMock.Object,
				_fileRepository,
				_blobStorageOptionsMock.Object,
				_jobSchedulerServiceMock.Object);

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
		[MemberData(nameof(WrongDateTimeToValidate))]
		public async Task CreateLotCommandHandler_WhenActiveCalledWithWrongDateProperties_ShouldHaveValidationErrors
			(DateTime startDate, DateTime endDate)
		{
			// Time: 2023-11-2T13:00:00
			var dateToTestOver = CustomDateTimeNow;

			var newLot = new CreateLotCommand
			{
				StartDate = startDate,
				EndDate = endDate,
				IsDraft = false
			};

			var result = await _validator.TestValidateAsync(newLot, default);

			if (startDate < dateToTestOver)
			{
				result.ShouldHaveValidationErrorFor(lot => lot.StartDate)
						.WithErrorCode("PredicateValidator");
			}
			result.ShouldHaveValidationErrorFor(lot => lot.EndDate)
				.WithErrorCode("PredicateValidator");
		}

		public static List<object[]> WrongDateTimeToValidate = new List<object[]>
		{
			new object[] { DateTime.MinValue.AddDays(1), DateTime.MinValue },
			new object[] { CustomDateTimeNow, CustomDateTimeNow },
			new object[] { CustomDateTimeNow.AddDays(1), CustomDateTimeNow },
			new object[] { CustomDateTimeNow.AddDays(1), CustomDateTimeNow.AddHours(2) },
		};
	}
}
