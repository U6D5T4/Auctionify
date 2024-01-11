using Auctionify.Application.Common.DTOs;
using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Common.Models.Requests;
using Auctionify.Core.Entities;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Auctionify.Application.Features.Users.Queries.GetTransactions
{
	public class GetTransactionsUserQuery
		: IRequest<GetListResponseDto<GetTransactionsUserResponse>>
	{
		public PageRequest PageRequest { get; set; }
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
			var role = (await _userManager.GetRolesAsync(user!)).FirstOrDefault()!;



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
