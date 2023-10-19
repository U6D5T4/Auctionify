using Auctionify.Application.Common.DTOs;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Common.Models.Requests;
using Auctionify.Core.Persistence.Dynamic;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Auctionify.Application.Features.Lots.Queries.GetAll
{
    public class GetAllLotsQuery : IRequest<GetListResponseDto<GetAllLotsResponse>>
    {
        public PageRequest PageRequest { get; set; }
    }

    public class GetAllLotsQueryHandler : IRequestHandler<GetAllLotsQuery, GetListResponseDto<GetAllLotsResponse>>
    {
        private readonly ILotRepository _lotRepository;
        private readonly IMapper _mapper;

        public GetAllLotsQueryHandler(ILotRepository lotRepository, IMapper mapper)
        {
            _lotRepository = lotRepository;
            _mapper = mapper;
        }

        public async Task<GetListResponseDto<GetAllLotsResponse>> Handle(GetAllLotsQuery request, CancellationToken cancellationToken)
        {
            var lots = await _lotRepository.GetListAsync(
                include: x => x.Include(l => l.Seller)
                                .Include(l => l.Location)
                                .Include(l => l.Category)
                                .Include(l => l.Currency)
                                .Include(l => l.LotStatus),
                enableTracking: false,
                size: request.PageRequest.PageSize,
                index: request.PageRequest.PageIndex,
                cancellationToken: cancellationToken);

            var response = _mapper.Map<GetListResponseDto<GetAllLotsResponse>>(lots);

            return response;
        }
    }
}
