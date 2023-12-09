using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Common.Options;
using Auctionify.Application.Features.Lots.Commands.Update;
using Auctionify.Core.Entities;
using Auctionify.Infrastructure.Persistence;
using Auctionify.Infrastructure.Repositories;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;

namespace Auctionify.UnitTests.UpdateLotTests
{
	public class UpdateLotTests : IDisposable
	{
		#region Initialization

		private readonly ILotRepository _lotRepository;
		private readonly IMapper _mapper;
		private readonly ILotStatusRepository _lotStatusRepository;
		private readonly IFileRepository _fileRepository;
		private readonly Mock<IBlobService> _blobServiceMock;
		private readonly Mock<IOptions<AzureBlobStorageOptions>> _blobStorageOptionsMock;
		private readonly UpdateLotCommandValidator _validator;
		private readonly Mock<IJobSchedulerService> _jobSchedulerServiceMock;

		public UpdateLotTests()
		{
			var mockDbContext = DbContextMock.GetMock<LotStatus, ApplicationDbContext>(
				EntitiesSeeding.GetLotStatuses(),
				ctx => ctx.LotStatuses
			);
			mockDbContext = DbContextMock.GetMock(
				EntitiesSeeding.GetCategories(),
				ctx => ctx.Categories,
				mockDbContext
			);
			mockDbContext = DbContextMock.GetMock(
				EntitiesSeeding.GetCurrencies(),
				ctx => ctx.Currency,
				mockDbContext
			);
			mockDbContext = DbContextMock.GetMock(
				EntitiesSeeding.GetLots(),
				ctx => ctx.Lots,
				mockDbContext
			);
			mockDbContext = DbContextMock.GetMock(
				EntitiesSeeding.GetFiles(),
				ctx => ctx.Files,
				mockDbContext
			);
			var blobStorageOptionsMock = new Mock<IOptions<AzureBlobStorageOptions>>();

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

			blobStorageOptionsMock
				.Setup(x => x.Value)
				.Returns(
					new AzureBlobStorageOptions
					{
						ContainerName = "auctionify-files",
						PhotosFolderName = "photos",
						AdditionalDocumentsFolderName = "additional-documents"
					}
				);

			_jobSchedulerServiceMock = new Mock<IJobSchedulerService>();

			_blobStorageOptionsMock = blobStorageOptionsMock;
			_lotRepository = new LotRepository(mockDbContext.Object);
			_lotStatusRepository = new LotStatusRepository(mockDbContext.Object);
			_fileRepository = new FileRepository(mockDbContext.Object);
			_blobServiceMock = new Mock<IBlobService>();
			_validator = new UpdateLotCommandValidator(
				_lotRepository,
				_lotStatusRepository,
				new CategoryRepository(mockDbContext.Object),
				new CurrencyRepository(mockDbContext.Object),
				_fileRepository
			);
		}

		#endregion

		#region Tests

		[Fact]
		public async Task Handle_ValidUpdate_ShouldUpdateLot()
		{
			var updateCommand = new UpdateLotCommand
			{
				Id = 1,
				Title = "Updated Lot Title",
				Description = "Updated Lot Description",
				StartingPrice = 150,
				StartDate = DateTime.Now,
				EndDate = DateTime.Now.AddDays(7),
				CategoryId = 2,
				City = "Updated City",
				Country = "Updated Country",
				Address = "Updated Address",
				CurrencyId = 2,
				IsDraft = false,
			};

			var handler = new UpdateLotCommandHandler(
				_lotRepository,
				_lotStatusRepository,
				_mapper,
				_blobServiceMock.Object,
				_fileRepository,
				_blobStorageOptionsMock.Object,
				_jobSchedulerServiceMock.Object
			);

			var result = await handler.Handle(updateCommand, default);

			result.Should().NotBeNull();
			result.Id.Should().Be(updateCommand.Id);
			result.Title.Should().Be(updateCommand.Title);
			result.Description.Should().Be(updateCommand.Description);
			result.StartingPrice.Should().Be(updateCommand.StartingPrice);
			result.StartDate.Should().Be(updateCommand.StartDate);
			result.EndDate.Should().Be(updateCommand.EndDate);
			result.Location.City.Should().Be(updateCommand.City);
			result.Location.Country.Should().Be(updateCommand.Country);
			result.Location.Address.Should().Be(updateCommand.Address);
		}

		[Fact]
		public async Task Handle_UpdateDraftLot_ShouldNotUpdatePhotosAndDocuments()
		{
			var updateCommand = new UpdateLotCommand
			{
				Id = 1,
				Title = "Updated Draft Lot",
				Description = "Updated description",
				StartingPrice = 100.0m,
				StartDate = DateTime.UtcNow,
				EndDate = DateTime.UtcNow.AddDays(7),
				CategoryId = 1,
				City = "New City",
				State = "New State",
				Country = "New Country",
				Address = "New Address",
				CurrencyId = 1,
				Photos = null,
				AdditionalDocuments = null,
				IsDraft = true,
			};

			var handler = new UpdateLotCommandHandler(
				_lotRepository,
				_lotStatusRepository,
				_mapper,
				_blobServiceMock.Object,
				_fileRepository,
				_blobStorageOptionsMock.Object,
				_jobSchedulerServiceMock.Object
			);

			var result = await handler.Handle(updateCommand, default);

			result.Should().NotBeNull();
			result.Id.Should().Be(updateCommand.Id);
			result.Title.Should().Be(updateCommand.Title);
			result.Description.Should().Be(updateCommand.Description);
			result.StartingPrice.Should().Be(updateCommand.StartingPrice);
			result.StartDate.Should().Be(updateCommand.StartDate);
			result.EndDate.Should().Be(updateCommand.EndDate);
			result.Location.City.Should().Be(updateCommand.City);
			result.Location.Country.Should().Be(updateCommand.Country);
			result.Location.Address.Should().Be(updateCommand.Address);
		}

		[Fact]
		public async Task Handle_UpdateNonDraftLot_ShouldUpdatePhotosAndDocuments()
		{
			var updateCommand = new UpdateLotCommand
			{
				Id = 1,
				Title = "Updated Lot",
				Description = "Updated description",
				StartingPrice = 100.0m,
				StartDate = DateTime.UtcNow,
				EndDate = DateTime.UtcNow.AddDays(7),
				CategoryId = 1,
				City = "New City",
				State = "New State",
				Country = "New Country",
				Address = "New Address",
				CurrencyId = 1,
				Photos = new List<IFormFile> { },
				AdditionalDocuments = new List<IFormFile> { },
				IsDraft = false,
			};

			var handler = new UpdateLotCommandHandler(
				_lotRepository,
				_lotStatusRepository,
				_mapper,
				_blobServiceMock.Object,
				_fileRepository,
				_blobStorageOptionsMock.Object,
				_jobSchedulerServiceMock.Object
			);

			var result = await handler.Handle(updateCommand, default);

			result.Should().NotBeNull();
			result.Id.Should().Be(updateCommand.Id);
			result.Title.Should().Be(updateCommand.Title);
			result.Description.Should().Be(updateCommand.Description);
			result.StartingPrice.Should().Be(updateCommand.StartingPrice);
			result.StartDate.Should().Be(updateCommand.StartDate);
			result.EndDate.Should().Be(updateCommand.EndDate);
			result.Location.City.Should().Be(updateCommand.City);
			result.Location.Country.Should().Be(updateCommand.Country);
			result.Location.Address.Should().Be(updateCommand.Address);
		}

		[Fact]
		public async Task Handle_NoPhotosProvidedForNonDraftLot_ShouldReturnValidationError()
		{
			var updateCommand = new UpdateLotCommand
			{
				Id = 2,
				Title = "Updated Lot",
				Description = "Updated descriptionUpdated descriptionUpdated descriptionUpdated descriptionUpdated description",
				StartingPrice = 100.0m,
				StartDate = DateTime.UtcNow.AddDays(2),
				EndDate = DateTime.UtcNow.AddDays(7),
				CategoryId = 1,
				City = "New City",
				State = "New State",
				Country = "New Country",
				Address = "New Address",
				CurrencyId = 1,
				Photos = null,
				AdditionalDocuments = null,
				IsDraft = false,
			};

			var result = await _validator.ValidateAsync(updateCommand);

			result.IsValid.Should().BeFalse();
			result.Errors
				.Should()
				.Contain(
					x => x.ErrorMessage == "At least 1 photo must be provided for lot creation"
				);
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
				_blobServiceMock.Reset();
				_blobStorageOptionsMock.Reset();
			}
		}

		#endregion
	}
}
