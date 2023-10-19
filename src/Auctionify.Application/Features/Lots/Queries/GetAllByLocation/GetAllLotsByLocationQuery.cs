using Auctionify.Application.Common.DTOs;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Core.Persistence.Dynamic;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Auctionify.Application.Features.Lots.Queries.GetAllByName
{
    public class GetAllLotsByLocationQuery : IRequest<GetListResponseDto<GetAllLotsByLocationResponse>>
    {
        public string Location { get; set; }
    }

    public class GetAllLotsByNameQueryHandler : IRequestHandler<GetAllLotsByLocationQuery, GetListResponseDto<GetAllLotsByLocationResponse>>
    {
        private readonly ILotRepository _lotRepository;
        private readonly IMapper _mapper;
        private readonly string namePropertyField = "Location.Country";
        private readonly string operatorPropertyField = "contains";

        public GetAllLotsByNameQueryHandler(ILotRepository lotRepository, IMapper mapper)
        {
            _lotRepository = lotRepository;
            _mapper = mapper;
        }

        public async Task<GetListResponseDto<GetAllLotsByLocationResponse>> Handle(GetAllLotsByLocationQuery request, CancellationToken cancellationToken)
        {
            var dynamicQuery = new DynamicQuery
            {
                Filter = new Filter
                {
                    Field = namePropertyField,
                    Operator = operatorPropertyField,
                    Value = request.Location
                }
            };

            var lots = await _lotRepository.GetListByDynamicAsync(dynamicQuery,
                include: x => x.Include(l => l.Seller)
                                .Include(l => l.Location)
                                .Include(l => l.Category)
                                .Include(l => l.Currency)
                                .Include(l => l.LotStatus),
                enableTracking: false,
                cancellationToken: cancellationToken);

            var response = _mapper.Map<GetListResponseDto<GetAllLotsByLocationResponse>>(lots);

            return response;
        }
    }

}