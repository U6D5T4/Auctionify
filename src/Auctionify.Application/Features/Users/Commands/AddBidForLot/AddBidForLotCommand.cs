using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Hubs;
using Auctionify.Core.Entities;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace Auctionify.Application.Features.Users.Commands.AddBidForLot
{
	public class AddBidForLotCommand : IRequest<AddedBidForLotResponse>
	{
		public int LotId { get; set; }
		public decimal Bid { get; set; }
	}

	public class AddBidForLotCommandHandler
		: IRequestHandler<AddBidForLotCommand, AddedBidForLotResponse>
	{
		private readonly IMapper _mapper;
		private readonly IBidRepository _bidRepository;
		private readonly ICurrentUserService _currentUserService;
		private readonly UserManager<User> _userManager;
		private readonly IHubContext<AuctionHub> _hubContext;
		private readonly ILotRepository _lotRepository;

		public AddBidForLotCommandHandler(
			IMapper mapper,
			IBidRepository bidRepository,
			ICurrentUserService currentUserService,
			UserManager<User> userManager,
			IHubContext<AuctionHub> hubContext,
			ILotRepository lotRepository
		)
		{
			_mapper = mapper;
			_bidRepository = bidRepository;
			_currentUserService = currentUserService;
			_userManager = userManager;
			_hubContext = hubContext;
			_lotRepository = lotRepository;
		}

		public async Task<AddedBidForLotResponse> Handle(
			AddBidForLotCommand request,
			CancellationToken cancellationToken
		)
		{
			var user = await _userManager.FindByEmailAsync(_currentUserService.UserEmail!);

			var lot = await _lotRepository.GetAsync(
				predicate: x => x.Id == request.LotId,
				cancellationToken: cancellationToken
			);

			if (lot is not null && request.Bid <= lot.StartingPrice)
			{
				throw new ValidationException("Bid must be greater than the lot's starting price");
			}

			var allUserBidsForLot = await _bidRepository.GetListAsync(
				predicate: x => x.LotId == request.LotId && x.BuyerId == user!.Id,
				orderBy: x => x.OrderByDescending(x => x.TimeStamp),
				cancellationToken: cancellationToken
			);

			if (allUserBidsForLot is not null && allUserBidsForLot.Items.Count > 0)
			{
				var recentBid = allUserBidsForLot.Items[0];

				if (request.Bid <= recentBid.NewPrice)
				{
					throw new ValidationException("Bid must be greater than the recent bid");
				}
			}

			var bid = new Bid
			{
				BuyerId = user!.Id,
				NewPrice = request.Bid,
				TimeStamp = DateTime.Now,
				LotId = request.LotId
			};

			var result = await _bidRepository.AddAsync(bid);

			await _hubContext.Clients.All.SendAsync("ReceiveBidNotification");

			var response = _mapper.Map<AddedBidForLotResponse>(result);

			return response;
		}
	}
}
