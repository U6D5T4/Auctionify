using Auctionify.Application.Common.DTOs;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Common.Models.Requests;
using Auctionify.Core.Enums;
using Auctionify.Core.Persistence.Dynamic;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Auctionify.Application.Features.Lots.Queries.GetAllByName
{
    public class GetAllLotsByNameQuery : IRequest<GetListResponseDto<GetAllLotsByNameResponse>>
    {
        public PageRequest PageRequest { get; set; }

        public string Name { get; set; }
    }

    public class GetAllLotsByNameQueryHandler : IRequestHandler<GetAllLotsByNameQuery, GetListResponseDto<GetAllLotsByNameResponse>>
    {
        private readonly ILotRepository _lotRepository;
        private readonly IMapper _mapper;
        private readonly string namePropertyField = "Title";
        private readonly string operatorPropertyField = "contains";

        private readonly List<string> validStatuses = new List<string>
        {
            AuctionStatus.Active.ToString(),
            AuctionStatus.Upcoming.ToString(),
        };

        public GetAllLotsByNameQueryHandler(ILotRepository lotRepository, IMapper mapper)
        {
            _lotRepository = lotRepository;
            _mapper = mapper;
        }

        public async Task<GetListResponseDto<GetAllLotsByNameResponse>> Handle(GetAllLotsByNameQuery request, CancellationToken cancellationToken)
        {
            var dynamicQuery = new DynamicQuery
            {
                Filter = new Filter
                {
                    Field = namePropertyField,
                    Operator = operatorPropertyField,
                    Value = request.Name
                }
            };

            var lots = await _lotRepository.GetListByDynamicAsync(dynamicQuery,
                predicate: l => validStatuses.Contains(l.LotStatus.Name),
                include: x => x.Include(l => l.Seller)
                                .Include(l => l.Location)
                                .Include(l => l.Category)
                                .Include(l => l.Currency)
                                .Include(l => l.LotStatus),
                enableTracking: false,
                size: request.PageRequest.PageSize,
                index: request.PageRequest.PageIndex,
                cancellationToken: cancellationToken);

            var response = _mapper.Map<GetListResponseDto<GetAllLotsByNameResponse>>(lots);

            return response;
        }
    }

}
