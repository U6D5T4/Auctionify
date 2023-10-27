using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Auctionify.Application.Features.Lots.Queries.GetAll
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
            var lots = await _lotRepository.GetListAsync(cancellationToken: cancellationToken);

			var response = _mapper.Map<List<GetAllLotsResponse>>(lots);

            return response;
        }
    }
}
