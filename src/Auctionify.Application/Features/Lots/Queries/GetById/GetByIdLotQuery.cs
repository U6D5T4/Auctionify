using Auctionify.Application.Common.Interfaces.Repositories;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
				var lot = await _lotRepository.GetAsync(predicate: x => x.Id == request.Id, include: x => x.Include(x => x.Category).Include(x => x.Currency).Include(x => x.Location).Include(x => x.LotStatus).Include(x => x.Bids));
				var result = _mapper.Map<GetByIdLotResponse>(lot);
				return result;
			}
		}

	}
}
