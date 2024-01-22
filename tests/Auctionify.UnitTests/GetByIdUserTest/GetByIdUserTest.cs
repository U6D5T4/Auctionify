﻿using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Features.Users.Queries.GetById;
using Auctionify.Core.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moq;
using Auctionify.Infrastructure.Persistence;
using Auctionify.Infrastructure.Repositories;
using Auctionify.Application.Common.Options;
using FluentAssertions;
using Microsoft.Extensions.Options;

namespace Auctionify.UnitTests.GetByIdUserTest
{
    public class GetByIdUserTests
    {
		#region Initialization

		private readonly IMapper _mapper;
		private readonly Mock<ICurrentUserService> _currentUserServiceMock;
		private readonly UserManager<User> _userManager;
		private readonly IRateRepository _rateRepository;

		public GetByIdUserTests() 
        {
			var mockDbContext = DbContextMock.GetMock<Rate, ApplicationDbContext>(EntitiesSeeding.GetRates(), ctx => ctx.Rates);
			mockDbContext = DbContextMock.GetMock(EntitiesSeeding.GetLotStatuses(), ctx => ctx.LotStatuses, mockDbContext);

			var configuration = new MapperConfiguration(
				cfg =>
					cfg.AddProfiles(
						new List<Profile>
						{
							new Application.Common.Profiles.MappingProfiles(),
							new Application.Features.Users.Profiles.MappingProfiles(),
						}
					)
			);
			_mapper = new Mapper(configuration);

			_userManager = EntitiesSeeding.GetUserManagerMock();

			_rateRepository = new RateRepository(mockDbContext.Object);
		}

		#endregion

		#region Test

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
				.Returns(new GetByIdUserResponse {  Id = userId, FirstName = "JohnDoe" });

			var query = new GetByIdUserQuery
			{
				Id = 1.ToString()
			};

			var testUrl = "test-url";
			var blobServiceMock = new Mock<IBlobService>();
			var azureBlobStorageOptionsMock = new Mock<IOptions<AzureBlobStorageOptions>>();

			azureBlobStorageOptionsMock
				.Setup(x => x.Value)
				.Returns(
					new AzureBlobStorageOptions
					{
						ContainerName = "auctionify-files",
						PhotosFolderName = "photos",
						AdditionalDocumentsFolderName = "additional-documents",
						UserProfilePhotosFolderName = "user-profile-photos"
					}
				);

			blobServiceMock
				.Setup(x => x.GetBlobUrl(It.IsAny<string>(), It.IsAny<string>()))
				.Returns(testUrl);

			var handler = new GetByIdUserQueryHandler(
				userManagerMock.Object,
				mapperMock.Object,
				blobServiceMock.Object,
				azureBlobStorageOptionsMock.Object,
				_rateRepository
			);

			// Act
			var result = await handler.Handle(query, default);

			Assert.NotNull(result);
            Assert.Equal(userId.ToString(), result.Id.ToString());
            Assert.Equal("JohnDoe", result.FirstName);
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
				_currentUserServiceMock.Reset();
			}
		}

		#endregion
	}
}