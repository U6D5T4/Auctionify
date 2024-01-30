using Auctionify.Application.Common.DTOs;
using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Common.Models.Requests;
using Auctionify.Core.Entities;
using Auctionify.Core.Persistence.Paging;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Auctionify.Application.Features.Users.Queries.GetByUserWatchlist
{
	public class GetWatchlistLotsQuery : IRequest<GetListResponseDto<GetWatchlistLotsResponse>>
	{
		public PageRequest PageRequest { get; set; }

		public GetWatchlistLotsQuery()
		{
			PageRequest = new PageRequest { PageIndex = 0, PageSize = 10 };
		}
	}

	public class GetWatchlistLotsQueryHandler
		: IRequestHandler<GetWatchlistLotsQuery, GetListResponseDto<GetWatchlistLotsResponse>>
	{
		private readonly IWatchlistRepository _watchlistRepository;
		private readonly ILocationRepository _locationRepository;
		private readonly ICategoryRepository _categoryRepository;
		private readonly ICurrencyRepository _currencyRepository;
		private readonly ILotStatusRepository _lotStatusRepository;
		private readonly IMapper _mapper;
		private readonly IPhotoService _photoService;
		private readonly ICurrentUserService _currentUserService;
		private readonly UserManager<User> _userManager;
		private readonly IBidRepository _bidRepository;

		public GetWatchlistLotsQueryHandler(
			IWatchlistRepository watchlistRepository,
			ILocationRepository locationRepository,
			ICategoryRepository categoryRepository,
			ICurrencyRepository currencyRepository,
			ILotStatusRepository lotStatusRepository,
			IMapper mapper,
			IPhotoService photoService,
			ICurrentUserService currentUserService,
			UserManager<User> userManager,
			IBidRepository bidRepository
		)
		{
			_watchlistRepository = watchlistRepository;
			_locationRepository = locationRepository;
			_categoryRepository = categoryRepository;
			_currencyRepository = currencyRepository;
			_lotStatusRepository = lotStatusRepository;
			_mapper = mapper;
			_photoService = photoService;
			_currentUserService = currentUserService;
			_userManager = userManager;
			_bidRepository = bidRepository;
		}

		public async Task<GetListResponseDto<GetWatchlistLotsResponse>> Handle(
			GetWatchlistLotsQuery request,
			CancellationToken cancellationToken
		)
		{
			var user = await _userManager.Users.FirstOrDefaultAsync(
				u => u.Email == _currentUserService.UserEmail! && !u.IsDeleted,
				cancellationToken: cancellationToken
			);

			var watchlist = await _watchlistRepository.GetUnpaginatedListAsync(
				predicate: x => x.UserId == user!.Id,
				include: x => x.Include(x => x.Lot),
				orderBy: x => x.OrderByDescending(x => x.Id),
				enableTracking: false,
				cancellationToken: cancellationToken
			);

			var lots = watchlist.Select(x => x.Lot).ToList();

			var paginatedLots = lots.Paginate(
				request.PageRequest.PageIndex,
				request.PageRequest.PageSize
			);

			var response = _mapper.Map<GetListResponseDto<GetWatchlistLotsResponse>>(paginatedLots);

			foreach (var lot in response.Items)
			{
				var location = await _locationRepository.GetAsync(
					predicate: x => x.Id == lot.LocationId,
					enableTracking: false,
					cancellationToken: cancellationToken
				);

				lot.Location = _mapper.Map<LocationDto>(location);

				var category = await _categoryRepository.GetAsync(
					predicate: x => x.Id == lot.CategoryId,
					enableTracking: false,
					cancellationToken: cancellationToken
				);

				lot.Category = _mapper.Map<CategoryDto>(category);

				var currency = await _currencyRepository.GetAsync(
					predicate: x => x.Id == lot.CurrencyId,
					enableTracking: false,
					cancellationToken: cancellationToken
				);

				lot.Currency = _mapper.Map<CurrencyDto>(currency);

				var lotStatus = await _lotStatusRepository.GetAsync(
					predicate: x => x.Id == lot.LotStatusId,
					enableTracking: false,
					cancellationToken: cancellationToken
				);

				lot.LotStatus = _mapper.Map<LotStatusDto>(lotStatus);

				lot.MainPhotoUrl = await _photoService.GetMainPhotoUrlAsync(
					lot.Id,
					cancellationToken
				);

				var bids = await _bidRepository.GetUnpaginatedListAsync(
					predicate: x => x.LotId == lot.Id && !x.BidRemoved,
					orderBy: x => x.OrderByDescending(x => x.TimeStamp),
					enableTracking: false,
					cancellationToken: cancellationToken
				);

				lot.BidCount = bids.Count;

				lot.Bids = _mapper.Map<List<BidDto>>(bids);

				lot.IsInWatchlist = true;

				lot.WatchlistId =
					watchlist.FirstOrDefault(x => x.LotId == lot.Id && x.UserId == user!.Id)?.Id
					?? 0;
			}

			return response;
		}
	}
}
