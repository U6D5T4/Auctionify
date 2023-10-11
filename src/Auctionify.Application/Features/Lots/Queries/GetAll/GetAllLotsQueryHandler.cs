using Auctionify.Application.Common.Interfaces;
using AutoMapper;
using MediatR;

namespace Auctionify.Application.Features.Lots.Queries.GetAllLots
{
    public class GetAllLotsQuery : IRequest<List<GetAllLotsResponse>>
    {

    }

    public class GetAllLotsQueryHandler : IRequestHandler<GetAllLotsQuery, List<GetAllLotsResponse>>
    {
        private readonly ILotRepository lotRepository;
        private readonly IMapper mapper;

        public GetAllLotsQueryHandler(ILotRepository lotRepository, IMapper mapper)
        {
            this.lotRepository = lotRepository;
            this.mapper = mapper;
        }

        public async Task<List<GetAllLotsResponse>> Handle(GetAllLotsQuery request, CancellationToken cancellationToken)
        {
            var lots = await lotRepository.GetListAsync();

            var response = mapper.Map<List<GetAllLotsResponse>>(lots);

            return response;
        }
    }
}
