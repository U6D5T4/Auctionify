using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using AutoMapper;
using MediatR;

namespace Auctionify.Application.Features.Lots.Queries.GetAllLots
{
    public class GetAllLotsQuery : IRequest<List<GetAllLotsResponse>>
    {

    }

    public class GetAllLotsQueryHandler : IRequestHandler<GetAllLotsQuery, List<GetAllLotsResponse>>
    {
        private readonly ILotRepository _lotRepository;
        private readonly IMapper _mapper;

        public GetAllLotsQueryHandler(ILotRepository lotRepository, IMapper mapper)
        {
            _lotRepository = lotRepository;
            _mapper = mapper;
        }

        public async Task<List<GetAllLotsResponse>> Handle(GetAllLotsQuery request, CancellationToken cancellationToken)
        {
            var lots = await _lotRepository.GetListAsync();

            var response = _mapper.Map<List<GetAllLotsResponse>>(lots);

            return response;
        }
    }
}
