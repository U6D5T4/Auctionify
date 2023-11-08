using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Features.Currencies.Queries.GetAll;
using Auctionify.Core.Entities;
using Auctionify.Infrastructure.Persistence;
using Auctionify.Infrastructure.Repositories;
using AutoMapper;
using FluentAssertions;

namespace Auctionify.UnitTests.Currencies.GetAllCurrenciesTests
{
	public class GetAllCurrenciesTests
	{
		private readonly ICurrencyRepository _currencyRepository;
		private readonly IMapper _mapper;

		public GetAllCurrenciesTests() {
			var dbContextMock = DbContextMock.GetMock<Currency, ApplicationDbContext>(EntitiesSeeding.GetCurrencies(), x => x.Currency);
			var configuration = new MapperConfiguration(cfg => cfg.AddProfiles(new List<Profile>
			{
				new Application.Common.Profiles.MappingProfiles(),
				new Application.Features.Currencies.Profiles.MappingProfiles(),
			}));

			_currencyRepository = new CurrencyRepository(dbContextMock.Object);
			_mapper = new Mapper(configuration);

		}

		[Fact]
		public async Task GetAllCurrenciesQueryHandler_WhenCalled_ReturnsAllSystemCurrencies()
		{
			var query = new GetAllCurrenciesQuery();
			var handler = new GetAllCurrenciesQueryHandler(_currencyRepository, _mapper);

			var result = await handler.Handle(query, default);

			result.Should().NotBeNull();
			result.Should().HaveCount(EntitiesSeeding.GetCurrencies().Count);
		}

		[Fact]
		public async Task GetAllCurrenciesQueryHandler_WhenCalledWithoutCurrenciesInSystem_ReturnsZeroItems()
		{
			var dbContextMock = DbContextMock.GetMock<Currency, ApplicationDbContext>(new List<Currency>(), x => x.Currency);
			var currencyRepository = new CurrencyRepository(dbContextMock.Object);

			var query = new GetAllCurrenciesQuery();
			var handler = new GetAllCurrenciesQueryHandler(currencyRepository, _mapper);

			var result = await handler.Handle(query, default);

			result.Should().HaveCount(0);
		}
	}
}
