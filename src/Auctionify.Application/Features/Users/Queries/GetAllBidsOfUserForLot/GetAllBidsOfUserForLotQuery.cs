using Auctionify.Application.Common.DTOs;
using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Common.Models.Requests;
using Auctionify.Core.Entities;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Auctionify.Application.Features.Users.Queries.GetAllBidsOfUserForLot
{
	public class GetAllBidsOfUserForLotQuery
		: IRequest<GetListResponseDto<GetAllBidsOfUserForLotResponse>>
	{
		public int LotId { get; set; }
		public PageRequest PageRequest { get; set; }
	}

	public class GetAllBidsOfUserForLotQueryHandler
		: IRequestHandler<
			GetAllBidsOfUserForLotQuery,
			GetListResponseDto<GetAllBidsOfUserForLotResponse>
		>
	{
		private readonly IMapper _mapper;
		private readonly IBidRepository _bidRepository;
		private readonly ILotRepository _lotRepository;
		private readonly ICurrencyRepository _currencyRepository;
		private readonly ICurrentUserService _currentUserService;
		private readonly UserManager<User> _userManager;

		public GetAllBidsOfUserForLotQueryHandler(
			IMapper mapper,
			IBidRepository bidRepository,
			ICurrentUserService currentUserService,
			UserManager<User> userManager,
			ILotRepository lotRepository,
			ICurrencyRepository currencyRepository
		)
		{
			_mapper = mapper;
			_bidRepository = bidRepository;
			_currentUserService = currentUserService;
			_userManager = userManager;
			_lotRepository = lotRepository;
			_currencyRepository = currencyRepository;
		}

		public async Task<GetListResponseDto<GetAllBidsOfUserForLotResponse>> Handle(
			GetAllBidsOfUserForLotQuery request,
			CancellationToken cancellationToken
		)
		{
			var user = await _userManager.FindByEmailAsync(_currentUserService.UserEmail!);

			var bids = await _bidRepository.GetListAsync(
				predicate: x => x.LotId == request.LotId && x.BuyerId == user!.Id && !x.BidRemoved,
				orderBy: x => x.OrderByDescending(x => x.TimeStamp),
				enableTracking: false,
				size: request.PageRequest.PageSize,
				index: request.PageRequest.PageIndex,
				cancellationToken: cancellationToken
			);

			var currency = string.Empty;

			var response = _mapper.Map<GetListResponseDto<GetAllBidsOfUserForLotResponse>>(bids);

			if (bids is not null)
			{
				var lot = await _lotRepository.GetAsync(
					predicate: x => x.Id == request.LotId,
					enableTracking: false,
					cancellationToken: cancellationToken
				);

				if (lot is not null)
				{
					var currencyEntity = await _currencyRepository.GetAsync(
						predicate: x => x.Id == lot.CurrencyId,
						enableTracking: false,
						cancellationToken: cancellationToken
					);

					if (currencyEntity is not null)
					{
						currency = currencyEntity.Code;

						foreach (var bid in response.Items)
						{
							bid.Currency = currency;
						}
					}
				}
			}

			return response;
		}
	}
}
