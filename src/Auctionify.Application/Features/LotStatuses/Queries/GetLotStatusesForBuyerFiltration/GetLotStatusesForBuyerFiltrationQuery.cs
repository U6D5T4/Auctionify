using Auctionify.Application.Common.Interfaces.Repositories;
using AutoMapper;
using MediatR;

namespace Auctionify.Application.Features.LotStatuses.Queries.GetLotStatusesForBuyerFiltration
{
    public class GetLotStatusesForBuyerFiltrationQuery : IRequest<IList<GetLotStatusesForBuyerFiltrationResponse>>
    {
    }

    public class GetLotStatusesForBuyerFiltrationQueryHandler : IRequestHandler<GetLotStatusesForBuyerFiltrationQuery, IList<GetLotStatusesForBuyerFiltrationResponse>>
    {
        private readonly ILotStatusRepository _lotStatusRepository;
        private readonly IMapper _mapper;
        private readonly IList<string> statusNames = new List<string>
        {
            "Closed",
            "Active",
            "Upcoming"
        };

        public GetLotStatusesForBuyerFiltrationQueryHandler(ILotStatusRepository lotStatusRepository,
            IMapper mapper)
        {
            _lotStatusRepository = lotStatusRepository;
            _mapper = mapper;
        }

        public async Task<IList<GetLotStatusesForBuyerFiltrationResponse>> Handle(GetLotStatusesForBuyerFiltrationQuery request, CancellationToken cancellationToken)
        {
            var result = await _lotStatusRepository.GetListAsync(predicate: s => statusNames.Contains(s.Name));

            return _mapper.Map<IList<GetLotStatusesForBuyerFiltrationResponse>>(result.Items);
        }
    }
}
