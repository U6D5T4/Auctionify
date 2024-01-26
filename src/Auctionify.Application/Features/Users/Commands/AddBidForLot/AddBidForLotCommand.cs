using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Hubs;
using Auctionify.Core.Entities;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

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

		public AddBidForLotCommandHandler(
			IMapper mapper,
			IBidRepository bidRepository,
			ICurrentUserService currentUserService,
			UserManager<User> userManager,
			IHubContext<AuctionHub> hubContext
		)
		{
			_mapper = mapper;
			_bidRepository = bidRepository;
			_currentUserService = currentUserService;
			_userManager = userManager;
			_hubContext = hubContext;
		}

		public async Task<AddedBidForLotResponse> Handle(
			AddBidForLotCommand request,
			CancellationToken cancellationToken
		)
		{
			var users = await _userManager.Users.ToListAsync(cancellationToken: cancellationToken);
			var user = users.Find(u => u.Email == _currentUserService.UserEmail! && !u.IsDeleted);

			var bid = new Bid
			{
				BuyerId = user!.Id,
				NewPrice = request.Bid,
				TimeStamp = DateTime.Now,
				LotId = request.LotId,
				BidRemoved = false
			};

			var result = await _bidRepository.AddAsync(bid);

			await _hubContext.Clients
				.Group(request.LotId.ToString())
				.SendAsync(
					SignalRActions.ReceiveBidNotification,
					cancellationToken: cancellationToken
				);

			var response = _mapper.Map<AddedBidForLotResponse>(result);

			return response;
		}
	}
}
