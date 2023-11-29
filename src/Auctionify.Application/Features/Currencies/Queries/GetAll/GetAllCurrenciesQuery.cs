using Auctionify.Application.Common.Interfaces.Repositories;
using AutoMapper;
using MediatR;

namespace Auctionify.Application.Features.Currencies.Queries.GetAll
{
	public class GetAllCurrenciesQuery : IRequest<IList<GetAllCurrenciesResponse>>
	{
	}

	public class GetAllCurrenciesQueryHandler : IRequestHandler<GetAllCurrenciesQuery, IList<GetAllCurrenciesResponse>>
	{
		private readonly ICurrencyRepository _currencyRepository;
		private readonly IMapper _mapper;

		public GetAllCurrenciesQueryHandler(ICurrencyRepository currencyRepository, IMapper mapper)
		{
			_currencyRepository = currencyRepository;
			_mapper = mapper;
		}

		public async Task<IList<GetAllCurrenciesResponse>> Handle(GetAllCurrenciesQuery request, CancellationToken cancellationToken)
		{
			var result = await _currencyRepository.GetListAsync(size: 50);

			return _mapper.Map<IList<GetAllCurrenciesResponse>>(result.Items);
		}
	}
}
