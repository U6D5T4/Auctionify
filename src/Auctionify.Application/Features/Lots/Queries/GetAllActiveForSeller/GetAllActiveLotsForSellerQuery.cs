using Auctionify.Application.Common.DTOs;
using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Common.Models.Requests;
using Auctionify.Core.Entities;
using Auctionify.Core.Enums;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Auctionify.Application.Features.Lots.Queries.GetAllActiveForSeller
{
	public class GetAllLotsWithStatusForSellerQuery :
		IRequest<GetListResponseDto<GetAllLotsWithStatusForSellerResponse>>
	{
		public AuctionStatus LotStatus { get; set; }
		public PageRequest PageRequest { get; set; }
	}

	public class GetAllLotsWithStatusForSellerQueryHandler :
		IRequestHandler<GetAllLotsWithStatusForSellerQuery, GetListResponseDto<GetAllLotsWithStatusForSellerResponse>>
	{
		private readonly ILotRepository _lotRepository;
		private readonly IMapper _mapper;
		private readonly ICurrentUserService _currentUserService;
		private readonly UserManager<User> _userManager;

		public GetAllLotsWithStatusForSellerQueryHandler(
			ILotRepository lotRepository,
			IMapper mapper,
			ICurrentUserService currentUserService,
			UserManager<User> userManager
			)
		{
			_lotRepository = lotRepository;
			_mapper = mapper;
			_currentUserService = currentUserService;
			_userManager = userManager;
		}

		public async Task<GetListResponseDto<GetAllLotsWithStatusForSellerResponse>> Handle(GetAllLotsWithStatusForSellerQuery request, CancellationToken cancellationToken)
		{
			var user = await _userManager.FindByEmailAsync(_currentUserService.UserEmail!);

			if (user == null)
			{
				throw new Exception("User not found!");
			}

			var activeLots = await _lotRepository.GetListAsync(
				predicate: x => x.SellerId == user.Id && x.LotStatus.Name.Contains(request.LotStatus.ToString()),
				include: x =>
					x.Include(l => l.Seller)
						.Include(l => l.Bids)
						.Include(l => l.Location)
						.Include(l => l.Category)
						.Include(l => l.Currency)
						.Include(l => l.LotStatus),
				size: request.PageRequest.PageSize,
				index: request.PageRequest.PageIndex,
				enableTracking: false,
				cancellationToken: cancellationToken);

			var response = _mapper.Map<GetListResponseDto<GetAllLotsWithStatusForSellerResponse>>(activeLots);

			foreach ( var lot in response.Items )
			{
				lot.BidCount = lot.Bids.Count;
			}

			return response;
		}
	}
}
