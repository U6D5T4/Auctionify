using Auctionify.Application.Common.DTOs;
using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Common.Models.Requests;
using Auctionify.Core.Entities;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Auctionify.Application.Features.Users.Queries.GetTransactions
{
	public class GetTransactionsUserQuery
		: IRequest<GetListResponseDto<GetTransactionsUserResponse>>
	{
		public PageRequest PageRequest { get; set; }

		public GetTransactionsUserQuery()
		{
			PageRequest = new PageRequest { PageIndex = 0, PageSize = 10 };
		}
	}

	public class GetTransactionsUserQueryHandler
		: IRequestHandler<GetTransactionsUserQuery, GetListResponseDto<GetTransactionsUserResponse>>
	{
		private readonly ILotRepository _lotRepository;
		private readonly IMapper _mapper;
		private readonly IPhotoService _photoService;
		private readonly ICurrentUserService _currentUserService;
		private readonly UserManager<User> _userManager;
		private readonly IBidRepository _bidRepository;

		public GetTransactionsUserQueryHandler(
			ILotRepository lotRepository,
			IMapper mapper,
			IPhotoService photoService,
			ICurrentUserService currentUserService,
			UserManager<User> userManager,
			IBidRepository bidRepository
		)
		{
			_lotRepository = lotRepository;
			_mapper = mapper;
			_photoService = photoService;
			_currentUserService = currentUserService;
			_userManager = userManager;
			_bidRepository = bidRepository;
		}

		public async Task<GetListResponseDto<GetTransactionsUserResponse>> Handle(
			GetTransactionsUserQuery request,
			CancellationToken cancellationToken
		)
		{
			var user = await _userManager.FindByEmailAsync(_currentUserService.UserEmail!);
			var role =
				(await _userManager.GetRolesAsync(user!)).FirstOrDefault()!
				?? throw new ValidationException("User does not have a role");

			var lots = await _lotRepository.GetListAsync(
				x => x.SellerId == user!.Id,
				include: x => x.Include(l => l.LotStatus),
				size: int.MaxValue,
				cancellationToken: cancellationToken
			);

			// logic for seller
			//foreach (var lot in lots.Items)
			//{
			//	var MainPhotoUrl = await _photoService.GetMainPhotoUrlAsync(
			//		lot.Id,
			//		cancellationToken
			//	);

			//	if (lot.LotStatus.Name == AuctionStatus.Sold.ToString())
			//	{
			//		var TransactionStatus = SellerStatus.Sold.ToString();

			//		var highestBid = (
			//			await _bidRepository.GetListAsync(
			//				predicate: x => x.LotId == lot.Id && !x.BidRemoved,
			//				orderBy: x => x.OrderByDescending(x => x.NewPrice),
			//				size: int.MaxValue,
			//				enableTracking: false,
			//				cancellationToken: cancellationToken
			//			)
			//		).Items.FirstOrDefault();

			//		var TransactionAmount = highestBid!.NewPrice;
			//		var TransactionDate = highestBid.TimeStamp;
			//	}
			//	else if (lot.LotStatus.Name == AuctionStatus.NotSold.ToString())
			//	{
			//		var TransactionStatus = SellerStatus.Expired.ToString();
			//		var TransactionAmount = lot.StartingPrice;
			//		var TransactionDate = lot.EndDate;
			//	}
			//	else if (lot.LotStatus.Name == AuctionStatus.Cancelled.ToString())
			//	{
			//		var TransactionStatus = SellerStatus.Cancelled.ToString();
			//		var TransactionAmount = lot.StartingPrice;
			//		var TransactionDate = lot.EndDate;
			//	}
			//}

			// logic for buyer


			//if (role == "Buyer")
			//{

			//}
			//else if (role == "Seller")
			//{

			//}

			throw new NotImplementedException();
		}
	}
}
