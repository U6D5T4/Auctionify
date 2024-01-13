using Auctionify.Application.Common.DTOs;
using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Common.Models.Requests;
using Auctionify.Application.Common.Models.Transaction;
using Auctionify.Core.Entities;
using Auctionify.Core.Enums;
using Auctionify.Core.Persistence.Paging;
using AutoMapper;
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
		private readonly IBidRepository _bidRepository;
		private readonly IMapper _mapper;
		private readonly IPhotoService _photoService;
		private readonly ICurrentUserService _currentUserService;
		private readonly UserManager<User> _userManager;

		public GetTransactionsUserQueryHandler(
			ILotRepository lotRepository,
			IBidRepository bidRepository,
			IMapper mapper,
			IPhotoService photoService,
			ICurrentUserService currentUserService,
			UserManager<User> userManager
		)
		{
			_lotRepository = lotRepository;
			_bidRepository = bidRepository;
			_mapper = mapper;
			_photoService = photoService;
			_currentUserService = currentUserService;
			_userManager = userManager;
		}

		public async Task<GetListResponseDto<GetTransactionsUserResponse>> Handle(
			GetTransactionsUserQuery request,
			CancellationToken cancellationToken
		)
		{
			var user = await _userManager.FindByEmailAsync(_currentUserService.UserEmail!);
			var role = (UserRole)
				Enum.Parse(
					typeof(UserRole),
					(await _userManager.GetRolesAsync(user!)).FirstOrDefault()!
				);

			var transactions = new List<TransactionInfo>();

			if (role == UserRole.Buyer)
			{
				var lotsForBuyer = await _lotRepository.GetUnpaginatedListAsync(
					include: x => x.Include(l => l.Currency).Include(l => l.LotStatus),
					cancellationToken: cancellationToken
				);

				foreach (var lot in lotsForBuyer)
				{
					var transactionBuyer = new TransactionInfo();

					if (lot.LotStatus.Name == AuctionStatus.Sold.ToString())
					{
						if (lot.BuyerId == user!.Id)
						{
							transactionBuyer.TransactionStatus =
								BuyerTransactionStatus.Winner.ToString();
						}
						else
						{
							transactionBuyer.TransactionStatus =
								BuyerTransactionStatus.Loss.ToString();
						}

						transactionBuyer.LotId = lot.Id;
						transactionBuyer.LotTitle = lot.Title;
						transactionBuyer.TransactionCurrency = lot.Currency!.Code;
						transactionBuyer.LotMainPhotoUrl = await _photoService.GetMainPhotoUrlAsync(
							lot.Id,
							cancellationToken
						);

						var highestBid = await GetHighestBidAsync(lot, cancellationToken, user.Id);

						transactionBuyer.TransactionAmount = highestBid!.NewPrice;
						transactionBuyer.TransactionDate = highestBid.TimeStamp;

						transactions.Add(transactionBuyer);
					}
					else if (
						lot.LotStatus.Name != AuctionStatus.Sold.ToString()
						|| lot.LotStatus.Name != AuctionStatus.NotSold.ToString()
						|| lot.LotStatus.Name != AuctionStatus.Cancelled.ToString()
					)
					{
						var withdrawnBid = await _bidRepository.GetAsync(
							predicate: x =>
								x.LotId == lot.Id && x.BidRemoved && x.BuyerId == user!.Id,
							enableTracking: false,
							cancellationToken: cancellationToken
						);

						if (withdrawnBid != null)
						{
							transactionBuyer.LotId = lot.Id;
							transactionBuyer.LotTitle = lot.Title;
							transactionBuyer.LotMainPhotoUrl =
								await _photoService.GetMainPhotoUrlAsync(lot.Id, cancellationToken);
							transactionBuyer.TransactionStatus =
								BuyerTransactionStatus.Withdraw.ToString();
							transactionBuyer.TransactionAmount = withdrawnBid!.NewPrice;
							transactionBuyer.TransactionCurrency = lot.Currency!.Code;
							transactionBuyer.TransactionDate = withdrawnBid.TimeStamp;

							transactions.Add(transactionBuyer);
						}
					}
				}
			}
			else if (role == UserRole.Seller)
			{
				var lotsForSeller = await _lotRepository.GetUnpaginatedListAsync(
					x => x.SellerId == user!.Id,
					include: x => x.Include(l => l.Currency).Include(l => l.LotStatus),
					cancellationToken: cancellationToken
				);

				foreach (var lot in lotsForSeller)
				{
					var transactionSeller = new TransactionInfo();

					if (lot.LotStatus.Name == AuctionStatus.Sold.ToString())
					{
						transactionSeller.LotId = lot.Id;
						transactionSeller.LotTitle = lot.Title;
						transactionSeller.LotMainPhotoUrl =
							await _photoService.GetMainPhotoUrlAsync(lot.Id, cancellationToken);
						transactionSeller.TransactionStatus =
							SellerTransactionStatus.Sold.ToString();
						transactionSeller.TransactionCurrency = lot.Currency!.Code;

						var highestBid = await GetHighestBidAsync(lot, cancellationToken);

						transactionSeller.TransactionAmount = highestBid!.NewPrice;
						transactionSeller.TransactionDate = highestBid.TimeStamp;

						transactions.Add(transactionSeller);
					}
					else if (
						lot.LotStatus.Name == AuctionStatus.NotSold.ToString()
						|| lot.LotStatus.Name == AuctionStatus.Cancelled.ToString()
					)
					{
						transactionSeller.TransactionStatus =
							lot.LotStatus.Name == AuctionStatus.NotSold.ToString()
								? SellerTransactionStatus.Expired.ToString()
								: SellerTransactionStatus.Cancelled.ToString();
						transactionSeller.LotId = lot.Id;
						transactionSeller.LotTitle = lot.Title;
						transactionSeller.LotMainPhotoUrl =
							await _photoService.GetMainPhotoUrlAsync(lot.Id, cancellationToken);
						transactionSeller.TransactionAmount = lot.StartingPrice;
						transactionSeller.TransactionCurrency = lot.Currency!.Code;
						transactionSeller.TransactionDate = lot.EndDate;

						transactions.Add(transactionSeller);
					}
				}
			}

			var sortedTransactions = transactions
				.OrderByDescending(x => x.TransactionDate)
				.ToList();

			var paginatedTransactions = sortedTransactions.Paginate(
				request.PageRequest.PageIndex,
				request.PageRequest.PageSize
			);

			var response = _mapper.Map<GetListResponseDto<GetTransactionsUserResponse>>(
				paginatedTransactions
			);

			return response;
		}

		private async Task<Bid> GetHighestBidAsync(
			Lot lot,
			CancellationToken cancellationToken,
			int BuyerId = 0
		)
		{
			if (BuyerId == 0) // for seller
			{
				var highestBid = (
					await _bidRepository.GetUnpaginatedListAsync(
						predicate: x => x.LotId == lot.Id && !x.BidRemoved,
						orderBy: x => x.OrderByDescending(x => x.NewPrice),
						enableTracking: false,
						cancellationToken: cancellationToken
					)
				).FirstOrDefault();

				return highestBid!;
			}
			else // for buyer
			{
				var highestBid = (
					await _bidRepository.GetUnpaginatedListAsync(
						predicate: x => x.LotId == lot.Id && !x.BidRemoved && x.BuyerId == BuyerId,
						orderBy: x => x.OrderByDescending(x => x.NewPrice),
						enableTracking: false,
						cancellationToken: cancellationToken
					)
				).FirstOrDefault();

				return highestBid!;
			}
		}
	}
}
