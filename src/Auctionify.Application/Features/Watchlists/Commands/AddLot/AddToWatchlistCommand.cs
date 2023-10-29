using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Core.Entities;
using AutoMapper;
using MediatR;

namespace Auctionify.Application.Features.Watchlists.Commands.AddLot
{
	public class AddToWatchlistCommand : IRequest<AddedToWatchlistResponse>
	{
		public int LotId { get; set; }
		public int UserId { get; set; }
	}

	public class AddToWatchListCommandHandler
		: IRequestHandler<AddToWatchlistCommand, AddedToWatchlistResponse>
	{
		private readonly IMapper _mapper;
		private readonly IWatchlistRepository _watchlistRepository;

		public AddToWatchListCommandHandler(
			IMapper mapper,
			IWatchlistRepository watchlistRepository
		)
		{
			_mapper = mapper;
			_watchlistRepository = watchlistRepository;
		}

		public async Task<AddedToWatchlistResponse> Handle(
			AddToWatchlistCommand request,
			CancellationToken cancellationToken
		)
		{
			var watchlist = new Watchlist { LotId = request.LotId, UserId = request.UserId };

			var result = await _watchlistRepository.AddAsync(watchlist);

			var response = _mapper.Map<AddedToWatchlistResponse>(result);

			return response;
		}
	}
}
