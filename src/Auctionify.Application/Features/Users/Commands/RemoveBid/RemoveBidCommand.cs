using Auctionify.Application.Common.Interfaces.Repositories;
using AutoMapper;
using MediatR;

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

		public RemoveBidCommandHandler(
			IMapper mapper,
			IBidRepository bidRepository
		)
		{
			_mapper = mapper;
			_bidRepository = bidRepository;
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

			await _bidRepository.DeleteAsync(bid!);

			var response = _mapper.Map<RemovedBidResponse>(bid);

			return response;
		}
	}
}
