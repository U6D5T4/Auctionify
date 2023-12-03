using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Hubs;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace Auctionify.Application.Features.Users.Commands.RemoveBid
{
	public class RemoveBidCommand : IRequest<RemovedBidResponse>
	{
		public int BidId { get; set; }
	}

	public class RemoveBidCommandHandler : IRequestHandler<RemoveBidCommand, RemovedBidResponse>
	{
		private readonly IMapper _mapper;
		private readonly IBidRepository _bidRepository;
		private readonly IHubContext<AuctionHub> _hubContext;

		public RemoveBidCommandHandler(
			IMapper mapper,
			IBidRepository bidRepository,
			IHubContext<AuctionHub> hubContext
		)
		{
			_mapper = mapper;
			_bidRepository = bidRepository;
			_hubContext = hubContext;
		}

		public async Task<RemovedBidResponse> Handle(
			RemoveBidCommand request,
			CancellationToken cancellationToken
		)
		{
			var bid = await _bidRepository.GetAsync(
				predicate: x => x.Id == request.BidId,
				cancellationToken: cancellationToken
			);

			bid!.BidRemoved = true;

			await _bidRepository.UpdateAsync(bid!);

			await _hubContext.Clients
				.Group(bid!.LotId.ToString())
				.SendAsync("ReceiveWithdrawBidNotification", cancellationToken: cancellationToken);

			var response = _mapper.Map<RemovedBidResponse>(bid);

			return response;
		}
	}
}
