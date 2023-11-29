using Auctionify.Application.Common.DTOs;
using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Common.Models.Requests;
using Auctionify.Core.Entities;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Auctionify.Application.Features.Users.Queries.GetByUserWatchlist
{
	public class GetWatchlistLotsQuery
		: IRequest<GetListResponseDto<GetWatchlistLotsResponse>>
	{
		public PageRequest PageRequest { get; set; }
	}

	public class GetWatchlistLotsQueryHandler
		: IRequestHandler<
			GetWatchlistLotsQuery,
			GetListResponseDto<GetWatchlistLotsResponse>
		>
	{
		private readonly IWatchlistRepository _watchlistRepository;
		private readonly ILotRepository _lotRepository;
		private readonly IMapper _mapper;
		private readonly IPhotoService _photoService;
		private readonly ICurrentUserService _currentUserService;
		private readonly UserManager<User> _userManager;
		private readonly IBidRepository _bidRepository;

		public GetWatchlistLotsQueryHandler(
			IWatchlistRepository watchlistRepository,
			ILotRepository lotRepository,
			IMapper mapper,
			IPhotoService photoService,
			ICurrentUserService currentUserService,
			UserManager<User> userManager,
			IBidRepository bidRepository
		)
		{
			_watchlistRepository = watchlistRepository;
			_lotRepository = lotRepository;
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
			var user = await _userManager.FindByEmailAsync(_currentUserService.UserEmail!);

			var watchlist = await _watchlistRepository.GetListAsync(
				predicate: x => x.UserId == user!.Id,
				include: x => x.Include(x => x.Lot),
				enableTracking: false,
				size: int.MaxValue,
				cancellationToken: cancellationToken
			);

			var lotsId = watchlist.Items.Select(x => x.LotId).ToList();

			var lots = await _lotRepository.GetListAsync(
				predicate: x => lotsId.Contains(x.Id),
				include: x =>
					x.Include(l => l.Seller)
						.Include(l => l.Location)
						.Include(l => l.Category)
						.Include(l => l.Currency)
						.Include(l => l.LotStatus),
				enableTracking: false,
				size: request.PageRequest.PageSize,
				index: request.PageRequest.PageIndex,
				cancellationToken: cancellationToken);

			var response = _mapper.Map<GetListResponseDto<GetWatchlistLotsResponse>>(lots);

			foreach (var lot in response.Items)
			{
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
	}
}
