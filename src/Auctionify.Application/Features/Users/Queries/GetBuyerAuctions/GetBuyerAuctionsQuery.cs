using Auctionify.Application.Common.DTOs;
using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Common.Models.Requests;
using Auctionify.Core.Entities;
using Auctionify.Core.Enums;
using Auctionify.Core.Persistence.Paging;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Auctionify.Application.Features.Users.Queries.GetBuyerAuctions
{
	public class GetBuyerAuctionsQuery : IRequest<GetListResponseDto<GetBuyerAuctionsResponse>>
	{
		public PageRequest PageRequest { get; set; }

		public GetBuyerAuctionsQuery()
		{
			PageRequest = new PageRequest { PageIndex = 0, PageSize = 10 };
		}
	}

	public class GetBuyerAuctionsQueryHandler
		: IRequestHandler<GetBuyerAuctionsQuery, GetListResponseDto<GetBuyerAuctionsResponse>>
	{
		private readonly ILotRepository _lotRepository;
		private readonly IBidRepository _bidRepository;
		private readonly IMapper _mapper;
		private readonly IPhotoService _photoService;
		private readonly IWatchlistService _watchlistService;
		private readonly ICurrentUserService _currentUserService;
		private readonly UserManager<User> _userManager;

		public GetBuyerAuctionsQueryHandler(
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

		public async Task<GetListResponseDto<GetBuyerAuctionsResponse>> Handle(
			GetBuyerAuctionsQuery request,
			CancellationToken cancellationToken
		)
		{
			var user = await _userManager.FindByEmailAsync(_currentUserService.UserEmail!);

			var bids = await _bidRepository.GetListAsync(
				predicate: x => x.BuyerId == user!.Id && !x.BidRemoved,
				enableTracking: false,
				size: int.MaxValue,
				cancellationToken: cancellationToken
			);

			var sortedBids = bids.Items.OrderByDescending(x => x.Id).ToList();

			var lotIds = sortedBids.Select(x => x.LotId).ToList();

			var lots = await _lotRepository.GetListAsync(
				predicate: x => lotIds.Contains(x.Id),
				include: x => x.Include(l => l.Currency).Include(l => l.LotStatus),
				enableTracking: false,
				size: int.MaxValue,
				cancellationToken: cancellationToken
			);

			var sortedLots = new List<Lot>();

			#region Sorting lots according to the order of bids (syncing) and removing duplicates

			foreach (var bid in sortedBids)
			{
				var lot = lots.Items.FirstOrDefault(x => x.Id == bid.LotId);

				if (lot != null)
				{
					sortedLots.Add(lot);
				}
			}

			var distinctLots = sortedLots.GroupBy(x => x.Id).Select(x => x.First());

			#endregion

			#region Pagination of distinct and sorted lots

			var paginatedLots = distinctLots.Paginate(
				request.PageRequest.PageIndex,
				request.PageRequest.PageSize
			);

			#endregion

			var response = _mapper.Map<GetListResponseDto<GetBuyerAuctionsResponse>>(paginatedLots);

			foreach (var lot in response.Items)
			{
				lot.IsInWatchlist = await _watchlistService.IsLotInUserWatchlist(
					lot.Id,
					user!.Id,
					cancellationToken
				);
				lot.MainPhotoUrl = await _photoService.GetMainPhotoUrlAsync(
					lot.Id,
					cancellationToken
				);

				var lotBids = await _bidRepository.GetListAsync(
					predicate: x => x.LotId == lot.Id && !x.BidRemoved,
					orderBy: x => x.OrderByDescending(x => x.TimeStamp),
					enableTracking: false,
					cancellationToken: cancellationToken
				);

				lot.BidCount = lotBids.Items.Count;

				lot.Bids = _mapper.Map<List<BidDto>>(lotBids.Items);
			}

			response.Items = response
				.Items.OrderByDescending(
					x => x.LotStatus.Name == AuctionStatus.Active.ToString() ? 1 : 0
				)
				.ThenByDescending(x => x.LotStatus.Name == AuctionStatus.Sold.ToString() ? 1 : 0)
				.ToList();

			return response;
		}
	}
}
