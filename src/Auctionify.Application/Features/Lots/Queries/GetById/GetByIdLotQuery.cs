using Auctionify.Application.Common.Interfaces.Repositories;
using AutoMapper;
using MediatR;

namespace Auctionify.Application.Features.Lots.Queries.GetById
{
	public class GetByIdLotQuery: IRequest<GetByIdLotResponse>
	{
		public int Id { get; set; }

		public class GetByIdLotQueryHandler : IRequestHandler<GetByIdLotQuery, GetByIdLotResponse>
		{
			private readonly ILotRepository _lotRepository;
			private readonly IMapper _mapper;

			public GetByIdLotQueryHandler(ILotRepository lotRepository, IMapper mapper)
			{
				_lotRepository = lotRepository;
				_mapper = mapper;
			}

			public async Task<GetByIdLotResponse> Handle(GetByIdLotQuery request, CancellationToken cancellationToken)
			{
				var lot = await _lotRepository.GetAsync(predicate: x => x.Id == request.Id, cancellationToken: cancellationToken);

				var result = _mapper.Map<GetByIdLotResponse>(lot);
				return result;
			}
		}

	}
}
