using Auctionify.Application.Common.DTOs;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Common.Models.Requests;
using Auctionify.Core.Entities;
using Auctionify.Core.Persistence.Dynamic;
using Auctionify.Core.Persistence.Paging;
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

        public string? SortField { get; set; }

        public string? SortDir { get; set; }

        public PageRequest? PageRequest { get; set; }
    }

    public class FilterLotsQueryHandler : IRequestHandler<FilterLotsQuery, GetListResponseDto<FilterLotsResponse>>
    {
        private string startingPriceField = "StartingPrice";
        private string categoryField = "CategoryId";
        private string lotStatusField = "LotStatusId";
        private string defaultOrder = "asc";
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
                var statusFiltersBase = CreateStatusFilter(request.LotStatuses, lotStatusField);
                filterBase.Filters.Add(statusFiltersBase);
            }

            if (request.MinimumPrice != null || request.MaximumPrice != null)
            {
                var priceBaseFilter = CreatePriceFilter(request.MinimumPrice, request.MaximumPrice, startingPriceField);
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

            Sort dynamicSort = null;

            if (!string.IsNullOrEmpty(request.SortField))
            {
                dynamicSort = new Sort
                {
                    Field = request.SortField,
                    Dir = !string.IsNullOrEmpty(request.SortDir) ? request.SortDir : defaultOrder
                };
            }

            var dynamicQuery = new DynamicQuery
            {
                Filter = filterBase,
                Sort = dynamicSort != null ? new List<Sort> { dynamicSort } : null,
            };

            var result = await (request.PageRequest != null
                ? _lotRepository.GetListByDynamicAsync(dynamicQuery, index: request.PageRequest.PageIndex, size: request.PageRequest.PageSize)
                : _lotRepository.GetListByDynamicAsync(dynamicQuery));

            return _mapper.Map<GetListResponseDto<FilterLotsResponse>>(result);
        }

        private Filter CreateStatusFilter(IList<int> statuses, string field)
        {
            var statusFiltersBase = new Filter
            {
                Field = lotStatusField,
                Logic = "and",
                Operator = "isnotnull",
                Filters = new List<Filter>(),
            };

            Filter? localFilter = null;

            for (int i = 0; i < statuses.Count; i++)
            {
                var statusFilter = new Filter
                {
                    Filters = new List<Filter>()
                };

                statusFilter.Field = lotStatusField;
                statusFilter.Logic = "or";
                statusFilter.Value = statuses[i].ToString();
                statusFilter.Operator = "eq";

                if (localFilter != null) statusFilter.Filters.Add(localFilter);

                localFilter = statusFilter;
            }

            statusFiltersBase.Filters.Add(localFilter);

            return statusFiltersBase;
        }

        Filter CreatePriceFilter(decimal? minPrice, decimal? maxPrice, string field)
        {
            var priceBaseFilter = new Filter
            {
                Field = field,
                Operator = "isnotnull",
                Logic = "and",
                Filters = new List<Filter>()
            };

            if (minPrice != null)
            {
                priceBaseFilter.Filters.Add(new Filter
                {
                    Field = field,
                    Value = minPrice.ToString(),
                    Operator = "gte",
                    Logic = "and",
                    Filters = new List<Filter>()
                });
            }

            if (maxPrice != null)
            {
                priceBaseFilter.Filters.Add(new Filter
                {
                    Field = field,
                    Value = maxPrice.ToString(),
                    Operator = "lte",
                    Logic = "and",
                    Filters = new List<Filter>()
                });
            }

            return priceBaseFilter;
        }
    }
}
