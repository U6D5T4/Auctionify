using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Features.Locations.Queries.GetAll;
using Auctionify.Core.Entities;
using Auctionify.Infrastructure.Persistence;
using Auctionify.Infrastructure.Repositories;
using AutoMapper;
using FluentAssertions;

namespace Auctionify.UnitTests.Locations.GetAllLocationsTests
{
	public class GetAllLocationsTests
	{
		private readonly IMapper _mapper;
		private readonly ILocationRepository _locationRepository;

		public GetAllLocationsTests()
		{
			var dbContextMock = DbContextMock.GetMock<Location, ApplicationDbContext>(EntitiesSeeding.GetLocations(), x => x.Locations);
			var configuration = new MapperConfiguration(cfg => cfg.AddProfiles(new List<Profile>
			{
				new Application.Common.Profiles.MappingProfiles(),
				new Application.Features.Currencies.Profiles.MappingProfiles(),
			}));

			_locationRepository = new LocationRepository(dbContextMock.Object);
			_mapper = new Mapper(configuration);
		}

		[Fact]
		public async Task GetAllLocationsQueryHandler_WhenCalled_ReturnsAllSystemLocations()
		{
			var query = new GetAllLocationsQuery();
			var handler = new GetAllLocationsQueryHandler(_locationRepository);

			var result = await handler.Handle(query, default);

			result.Should().NotBeNull();
			result.Should().HaveCount(EntitiesSeeding.GetLocations().Count);
		}

		[Fact]
		public async Task GetAllLocationsQueryHandler_WhenCalledWithoutLocationsInSystem_ReturnsZeroItems()
		{
			var dbContextMock = DbContextMock.GetMock<Location, ApplicationDbContext>(new List<Location>(), x => x.Locations);
			var locationRepository = new LocationRepository(dbContextMock.Object);

			var query = new GetAllLocationsQuery();
			var handler = new GetAllLocationsQueryHandler(locationRepository);

			var result = await handler.Handle(query, default);

			result.Should().HaveCount(0);
		}
	}
}
