using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Core.Entities;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Auctionify.Application.Features.Users.Commands.AddLotToWatchlist
{
	public class AddToWatchlistCommand : IRequest<AddedToWatchlistResponse>
	{
		public int LotId { get; set; }
	}

	public class AddToWatchListCommandHandler
		: IRequestHandler<AddToWatchlistCommand, AddedToWatchlistResponse>
	{
		private readonly IMapper _mapper;
		private readonly IWatchlistRepository _watchlistRepository;
		private readonly ICurrentUserService _currentUserService;
		private readonly UserManager<User> _userManager;

		public AddToWatchListCommandHandler(
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

		public async Task<AddedToWatchlistResponse> Handle(
			AddToWatchlistCommand request,
			CancellationToken cancellationToken
		)
		{
			var users = await _userManager.Users.ToListAsync(cancellationToken: cancellationToken);
			var user = users.Find(u => u.Email == _currentUserService.UserEmail! && !u.IsDeleted);

			var watchlist = new Watchlist { LotId = request.LotId, UserId = user!.Id };

			var result = await _watchlistRepository.AddAsync(watchlist);

			var response = _mapper.Map<AddedToWatchlistResponse>(result);

			return response;
		}
	}
}
