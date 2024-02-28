using Auctionify.Application.Common.DTOs;
using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Common.Models.Requests;
using Auctionify.Core.Entities;
using Auctionify.Core.Enums;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Auctionify.Application.Features.Lots.Queries.GetAll
{
	public class GetAllLotsQuery : IRequest<GetListResponseDto<GetAllLotsResponse>>
	{
		public PageRequest PageRequest { get; set; }
	}

	public class GetAllLotsQueryHandler
		: IRequestHandler<GetAllLotsQuery, GetListResponseDto<GetAllLotsResponse>>
	{
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

		public GetAllLotsQueryHandler(
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

		public async Task<GetListResponseDto<GetAllLotsResponse>> Handle(
			GetAllLotsQuery request,
			CancellationToken cancellationToken
		)
		{
			var user = await _userManager.Users.FirstOrDefaultAsync(
				u => u.Email == _currentUserService.UserEmail! && !u.IsDeleted,
				cancellationToken: cancellationToken
			);

			var lots = await _lotRepository.GetListAsync(
				predicate: x => validStatuses.Contains(x.LotStatus.Name),
				include: x =>
					x.Include(l => l.Seller)
						.Include(l => l.Location)
						.Include(l => l.Category)
						.Include(l => l.Currency)
						.Include(l => l.LotStatus),
				enableTracking: false,
				size: request.PageRequest.PageSize,
				index: request.PageRequest.PageIndex,
				cancellationToken: cancellationToken
			);

			var response = _mapper.Map<GetListResponseDto<GetAllLotsResponse>>(lots);

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
