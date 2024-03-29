﻿using Auctionify.Application.Common.DTOs;
using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Common.Models.Requests;
using Auctionify.Core.Entities;
using Auctionify.Core.Enums;
using Auctionify.Core.Persistence.Dynamic;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Auctionify.Application.Features.Lots.Queries.Filter
{
	public class FilterLotsQuery : IRequest<GetListResponseDto<FilterLotsResponse>>
	{
		public decimal? MinimumPrice { get; set; }

		public decimal? MaximumPrice { get; set; }

		public int? CategoryId { get; set; }

		public string? Location { get; set; }

		public IList<int>? LotStatuses { get; set; }

		public string? SortField { get; set; }

		public string? SortDir { get; set; }

		public PageRequest? PageRequest { get; set; }
	}

	public class FilterLotsQueryHandler
		: IRequestHandler<FilterLotsQuery, GetListResponseDto<FilterLotsResponse>>
	{
		private const string startingPriceField = nameof(Lot.StartingPrice);
		private const string categoryField = nameof(Lot.CategoryId);
		private const string lotStatusField = nameof(Lot.LotStatusId);
		private const string locationField = $"{nameof(Lot.Location)}.City";
		private const string defaultOrder = "asc";
		private readonly ILotRepository _lotRepository;
		private readonly IMapper _mapper;
		private readonly IPhotoService _photoService;
		private readonly ICurrentUserService _currentUserService;
		private readonly UserManager<User> _userManager;
		private readonly IWatchlistService _watchlistService;
		private readonly IBidRepository _bidRepository;
		private readonly List<string> validStatuses =
			new()
			{
				AuctionStatus.Active.ToString(),
				AuctionStatus.Upcoming.ToString(),
				AuctionStatus.Archive.ToString(),
				AuctionStatus.Cancelled.ToString(),
				AuctionStatus.Sold.ToString(),
				AuctionStatus.NotSold.ToString(),
			};

		public FilterLotsQueryHandler(
			ILotRepository lotRepository,
			IMapper mapper,
			IPhotoService photoService,
			ICurrentUserService currentUserService,
			UserManager<User> userManager,
			IWatchlistService watchlistService,
			IBidRepository bidRepository
		)
		{
			_lotRepository = lotRepository;
			_mapper = mapper;
			_photoService = photoService;
			_currentUserService = currentUserService;
			_userManager = userManager;
			_watchlistService = watchlistService;
			_bidRepository = bidRepository;
		}

		public async Task<GetListResponseDto<FilterLotsResponse>> Handle(
			FilterLotsQuery request,
			CancellationToken cancellationToken
		)
		{
			var user = await _userManager.Users.FirstOrDefaultAsync(
				u => u.Email == _currentUserService.UserEmail! && !u.IsDeleted,
				cancellationToken: cancellationToken
			);

			var filterBase = new Core.Persistence.Dynamic.Filter
			{
				Field = "Id",
				Operator = "isnotnull",
				Logic = "and",
				Filters = new List<Core.Persistence.Dynamic.Filter>()
			};

			if (request.LotStatuses != null)
			{
				var statusFiltersBase = CreateStatusFilter(request.LotStatuses, lotStatusField);
				filterBase.Filters.Add(statusFiltersBase);
			}

			IQueryable<Lot>? queryableForLotPrice = null;

			if (request.MinimumPrice != null || request.MaximumPrice != null)
			{
				queryableForLotPrice = _lotRepository.Query();

				queryableForLotPrice = CreatePriceFilter(
					queryableForLotPrice,
					request.MinimumPrice,
					request.MaximumPrice,
					startingPriceField
				);
			}

			if (request.CategoryId != null)
			{
				var filterCategory = new Core.Persistence.Dynamic.Filter
				{
					Field = categoryField,
					Value = request.CategoryId.ToString(),
					Operator = "eq",
					Logic = "and",
					Filters = new List<Core.Persistence.Dynamic.Filter>()
				};

				filterBase.Filters.Add(filterCategory);
			}

			if (request.Location != null)
			{
				var filterLocation = new Core.Persistence.Dynamic.Filter
				{
					Field = locationField,
					Value = request.Location,
					Operator = "contains",
					Logic = "and",
					Filters = new List<Core.Persistence.Dynamic.Filter>()
				};

				filterBase.Filters.Add(filterLocation);
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

			var result = await (
				request.PageRequest != null
					? _lotRepository.GetListByDynamicAsync(
						dynamicQuery,
						existingQueryable: queryableForLotPrice,
						predicate: l => validStatuses.Contains(l.LotStatus.Name),
						include: x =>
							x.Include(l => l.Location)
								.Include(l => l.Category)
								.Include(l => l.Currency)
								.Include(l => l.LotStatus),
						index: request.PageRequest.PageIndex,
						size: request.PageRequest.PageSize,
						cancellationToken: cancellationToken
					)
					: _lotRepository.GetListByDynamicAsync(
						dynamicQuery,
						existingQueryable: queryableForLotPrice,
						predicate: l => validStatuses.Contains(l.LotStatus.Name),
						include: x =>
							x.Include(l => l.Category)
								.Include(l => l.Location)
								.Include(l => l.Currency)
								.Include(l => l.LotStatus),
						cancellationToken: cancellationToken
					)
			);

			var response = _mapper.Map<GetListResponseDto<FilterLotsResponse>>(result);

			foreach (var lot in response.Items)
			{
				if (user is not null)
				{
					lot.IsInWatchlist = await _watchlistService.IsLotInUserWatchlist(
						lot.Id,
						user!.Id,
						cancellationToken
					);
				}

				lot.MainPhotoUrl = await _photoService.GetMainPhotoUrlAsync(
					lot.Id,
					cancellationToken
				);

				var bids = await _bidRepository.GetListAsync(
					predicate: x => x.LotId == lot.Id && !x.BidRemoved,
					orderBy: x => x.OrderByDescending(x => x.TimeStamp),
					enableTracking: false,
					cancellationToken: cancellationToken
				);

				lot.BidCount = bids.Items.Count;

				lot.Bids = _mapper.Map<List<BidDto>>(bids.Items);
			}

			return response;
		}

		private Core.Persistence.Dynamic.Filter CreateStatusFilter(
			IList<int> statuses,
			string field
		)
		{
			var statusFiltersBase = new Core.Persistence.Dynamic.Filter
			{
				Field = lotStatusField,
				Logic = "and",
				Operator = "isnotnull",
				Filters = new List<Core.Persistence.Dynamic.Filter>(),
			};

			Core.Persistence.Dynamic.Filter? localFilter = null;

			foreach (var status in statuses)
			{
				var statusFilter = new Core.Persistence.Dynamic.Filter
				{
					Filters = new List<Core.Persistence.Dynamic.Filter>(),
					Field = lotStatusField,
					Logic = "or",
					Value = status.ToString(),
					Operator = "eq",
				};

				if (localFilter != null)
					statusFilter.Filters.Add(localFilter);

				localFilter = statusFilter;
			}

			statusFiltersBase.Filters.Add(localFilter);

			return statusFiltersBase;
		}

		private IQueryable<Lot> CreatePriceFilter(
			IQueryable<Lot> queryable,
			decimal? minPrice,
			decimal? maxPrice,
			string field
		)
		{
			queryable = queryable.Include(l => l.Bids);

			if (minPrice != null)
			{
				queryable = queryable.Where(
					lot =>
						(
							lot.Bids.Count() > 0
							&& lot.Bids.OrderByDescending(b => b.NewPrice).First().NewPrice
								>= minPrice
						) || (lot.Bids.Count() == 0 && lot.StartingPrice >= minPrice)
				);
			}

			if (maxPrice != null)
			{
				queryable = queryable.Where(
					lot =>
						(
							lot.Bids.Count() > 0
							&& lot.Bids.OrderByDescending(b => b.NewPrice).First().NewPrice
								<= maxPrice
						) || (lot.Bids.Count() == 0 && lot.StartingPrice <= maxPrice)
				);
			}

			var str = queryable.ToQueryString();

			return queryable;
		}
	}
}
