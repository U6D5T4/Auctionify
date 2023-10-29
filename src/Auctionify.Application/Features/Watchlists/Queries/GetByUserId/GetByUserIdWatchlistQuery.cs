using Auctionify.Application.Common.DTOs;
using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Common.Models.Requests;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Auctionify.Application.Features.Watchlists.Queries.GetByUserId
{
	public class GetByUserIdWatchlistQuery
		: IRequest<GetListResponseDto<GetByUserIdWatchlistResponse>>
	{
		public int UserId { get; set; }
		public PageRequest PageRequest { get; set; }
	}

	public class GetByUserIdWatchlistQueryHandler
		: IRequestHandler<
			GetByUserIdWatchlistQuery,
			GetListResponseDto<GetByUserIdWatchlistResponse>
		>
	{
		private readonly IWatchlistRepository _watchlistRepository;
		private readonly ILotRepository _lotRepository;
		private readonly IMapper _mapper;
		private readonly IPhotoService _photoService;

		public GetByUserIdWatchlistQueryHandler(
			IWatchlistRepository watchlistRepository,
			ILotRepository lotRepository,
			IMapper mapper,
			IPhotoService photoService
		)
		{
			_watchlistRepository = watchlistRepository;
			_lotRepository = lotRepository;
			_mapper = mapper;
			_photoService = photoService;
		}

		public async Task<GetListResponseDto<GetByUserIdWatchlistResponse>> Handle(
			GetByUserIdWatchlistQuery request,
			CancellationToken cancellationToken
		)
		{
			var watchlist = await _watchlistRepository.GetListAsync(
				predicate: x => x.UserId == request.UserId,
				include: x => x.Include(x => x.Lot),
				enableTracking: false,
				size: request.PageRequest.PageSize,
				index: request.PageRequest.PageIndex,
				cancellationToken: cancellationToken
			);

			var lots = watchlist.Items.Select(x => x.Lot).ToList();
			
			var response = _mapper.Map<GetListResponseDto<GetByUserIdWatchlistResponse>>(lots);

			foreach (var lot in response.Items)
			{
				lot.MainPhotoUrl = await _photoService.GetMainPhotoUrlAsync(
					lot.Id,
					cancellationToken
				);
			}

			return response;
		}
	}
}
