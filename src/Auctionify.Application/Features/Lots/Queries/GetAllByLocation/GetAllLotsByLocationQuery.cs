using Auctionify.Application.Common.DTOs;
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

namespace Auctionify.Application.Features.Lots.Queries.GetAllByName
{
	public class GetAllLotsByLocationQuery
		: IRequest<GetListResponseDto<GetAllLotsByLocationResponse>>
	{
		public PageRequest PageRequest { get; set; }
		public string Location { get; set; }
	}

	public class GetAllLotsByLocationQueryHandler
		: IRequestHandler<
			GetAllLotsByLocationQuery,
			GetListResponseDto<GetAllLotsByLocationResponse>
		>
	{
		private readonly ILotRepository _lotRepository;
		private readonly ICurrentUserService _currentUserService;
		private readonly UserManager<User> _userManager;
		private readonly IWatchlistService _watchlistService;
		private readonly IPhotoService _photoService;
		private readonly IMapper _mapper;
		private readonly IBidRepository _bidRepository;
		private readonly string namePropertyField = "Location.City";
		private readonly string operatorPropertyField = "contains";
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

		public GetAllLotsByLocationQueryHandler(
			ILotRepository lotRepository,
			ICurrentUserService currentUserService,
			UserManager<User> userManager,
			IWatchlistService watchlistService,
			IPhotoService photoService,
			IMapper mapper,
			IBidRepository bidRepository
		)
		{
			_lotRepository = lotRepository;
			_currentUserService = currentUserService;
			_userManager = userManager;
			_watchlistService = watchlistService;
			_photoService = photoService;
			_mapper = mapper;
			_bidRepository = bidRepository;
		}

		public async Task<GetListResponseDto<GetAllLotsByLocationResponse>> Handle(
			GetAllLotsByLocationQuery request,
			CancellationToken cancellationToken
		)
		{
			var user = await _userManager.Users.FirstOrDefaultAsync(
				u => u.Email == _currentUserService.UserEmail! && !u.IsDeleted,
				cancellationToken: cancellationToken
			);

			var dynamicQuery = new DynamicQuery
			{
				Filter = new Core.Persistence.Dynamic.Filter
				{
					Field = namePropertyField,
					Operator = operatorPropertyField,
					Value = request.Location
				}
			};

			var lots = await _lotRepository.GetListByDynamicAsync(
				dynamicQuery,
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

			var response = _mapper.Map<GetListResponseDto<GetAllLotsByLocationResponse>>(lots);

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
