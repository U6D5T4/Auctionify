using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Core.Entities;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Auctionify.Application.Features.Users.Commands.RemoveLotFromWatchlist
{
	public class RemoveLotFromWatchlistCommand : IRequest<RemovedLotFromWatchlistResponse>
	{
		public int LotId { get; set; }
	}

	public class RemoveLotFromWatchlistCommandHandler
		: IRequestHandler<RemoveLotFromWatchlistCommand, RemovedLotFromWatchlistResponse>
	{
		private readonly IMapper _mapper;
		private readonly IWatchlistRepository _watchlistRepository;
		private readonly ICurrentUserService _currentUserService;
		private readonly UserManager<User> _userManager;

		public RemoveLotFromWatchlistCommandHandler(
			IMapper mapper,
			IWatchlistRepository watchlistRepository,
			ICurrentUserService currentUserService,
			UserManager<User> userManager
		)
		{
			_mapper = mapper;
			_watchlistRepository = watchlistRepository;
			_currentUserService = currentUserService;
			_userManager = userManager;
		}

		public async Task<RemovedLotFromWatchlistResponse> Handle(
			RemoveLotFromWatchlistCommand request,
			CancellationToken cancellationToken
		)
		{
			var user = await _userManager.Users.FirstOrDefaultAsync(
				u => u.Email == _currentUserService.UserEmail! && !u.IsDeleted,
				cancellationToken: cancellationToken
			);

			var watchlist = await _watchlistRepository.GetAsync(
				predicate: w => w.UserId == user!.Id && w.LotId == request.LotId,
				cancellationToken: cancellationToken
			);

			var result = await _watchlistRepository.DeleteAsync(watchlist!);

			var response = _mapper.Map<RemovedLotFromWatchlistResponse>(result);

			return response;
		}
	}
}
