using Auctionify.Application.Common.DTOs;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Core.Persistence.Dynamic;
using AutoMapper;
using MediatR;
using System.Linq;

namespace Auctionify.Application.Features.Lots.Queries.FIlter
{
    public class FilterLotsQuery : IRequest<GetListResponseDto<FilterLotsResponse>>
    {
        public decimal? MinimumPrice { get; set; }

        public decimal? MaximumPrice { get; set; }

        public int? CategoryId { get; set; }

        public IList<int>? LotStatuses { get; set; }

        public string? Sort { get; set; }
    }

    public class FilterLotsQueryHandler : IRequestHandler<FilterLotsQuery, GetListResponseDto<FilterLotsResponse>>
    {
        private string startingPriceField = "StartingPrice";
        private string categoryField = "CategoryId";
        private string lotStatusField = "LotStatusId";
        private readonly ILotRepository _lotRepository;
        private readonly IMapper _mapper;

        public FilterLotsQueryHandler(ILotRepository lotRepository,
            IMapper mapper)
        {
            _lotRepository = lotRepository;
            _mapper = mapper;
        }

        public async Task<GetListResponseDto<FilterLotsResponse>> Handle(FilterLotsQuery request, CancellationToken cancellationToken)
        {
            var filterBase = new Filter
            {
                Field = "Id",
                Operator = "isnotnull",
                Logic = "and",
                Filters = new List<Filter>()
            };

            if (request.LotStatuses != null)
            {
                var statusFiltersBase = new Filter
                {
                    Field = lotStatusField,
                    Logic = "and",
                    Operator = "isnotnull",
                    Filters = new List<Filter>(),
                };

                Filter? localFilter = null;

                for (int i = 0; i < request.LotStatuses.Count; i++)
                {
                    var statusFilter = new Filter
                    {
                        Filters = new List<Filter>()
                    };

                    statusFilter.Field = lotStatusField;
                    statusFilter.Logic = "or";
                    statusFilter.Value = request.LotStatuses[i].ToString();
                    statusFilter.Operator = "eq";

                    if (localFilter != null) statusFilter.Filters.Add(localFilter);

                    localFilter = statusFilter;
                }
                statusFiltersBase.Filters.Add(localFilter);
                filterBase.Filters.Add(statusFiltersBase);
            }

            if (request.MinimumPrice != null || request.MaximumPrice != null)
            {
                var priceBaseFilter = new Filter
                {
                    Field = startingPriceField,
                    Operator = "isnotnull",
                    Logic = "and",
                    Filters = new List<Filter>()
                };

                if (request.MinimumPrice != null)
                {
                    var filterMiniumPrice = new Filter
                    {
                        Field = startingPriceField,
                        Value = request.MinimumPrice.ToString(),
                        Operator = "gte",
                        Logic = "and",
                        Filters = new List<Filter>()
                    };

                    priceBaseFilter.Filters.Add(filterMiniumPrice);
                }

                if (request.MaximumPrice != null)
                {
                    var filterMaximumPrice = new Filter
                    {
                        Field = startingPriceField,
                        Value = request.MaximumPrice.ToString(),
                        Operator = "lte",
                        Logic = "and",
                        Filters = new List<Filter>()
                    };

                    priceBaseFilter.Filters.Add(filterMaximumPrice);
                }

                filterBase.Filters.Add(priceBaseFilter);
            }

            if (request.CategoryId != null)
            {
                var filterCategory = new Filter
                {
                    Field = categoryField,
                    Value = request.CategoryId.ToString(),
                    Operator = "eq",
                    Logic = "and",
                    Filters = new List<Filter>()
                };

                filterBase.Filters.Add(filterCategory);
            }

            var dynamicQuery = new DynamicQuery
            {
                Filter = filterBase,
            };

            var result = await _lotRepository.GetListByDynamicAsync(dynamicQuery);

            return _mapper.Map<GetListResponseDto<FilterLotsResponse>>(result);
        }
    }
}
