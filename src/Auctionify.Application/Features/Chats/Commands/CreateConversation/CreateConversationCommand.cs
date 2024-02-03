using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Core.Entities;
using Auctionify.Core.Enums;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Auctionify.Application.Features.Chats.Commands.CreateConversation
{
	public class CreateConversationCommand : IRequest<CreatedConversationResponse>
	{
		public int LotId { get; set; }
	}

	public class CreateConversationCommandHandler
		: IRequestHandler<CreateConversationCommand, CreatedConversationResponse>
	{
		private readonly IMapper _mapper;
		private readonly ILotRepository _lotRepository;
		private readonly IConversationRepository _conversationRepository;
		private readonly ICurrentUserService _currentUserService;
		private readonly UserManager<User> _userManager;

		public CreateConversationCommandHandler(
			IMapper mapper,
			ILotRepository lotRepository,
			IConversationRepository conversationRepository,
			ICurrentUserService currentUserService,
			UserManager<User> userManager
		)
		{
			_mapper = mapper;
			_lotRepository = lotRepository;
			_conversationRepository = conversationRepository;
			_currentUserService = currentUserService;
			_userManager = userManager;
		}

		public async Task<CreatedConversationResponse> Handle(
			CreateConversationCommand request,
			CancellationToken cancellationToken
		)
		{
			var currentUser = await _userManager.Users.FirstOrDefaultAsync(
				u => u.Email == _currentUserService.UserEmail! && !u.IsDeleted,
				cancellationToken: cancellationToken
			);

			var currentUserRole = (AccountRole)
				Enum.Parse(
					typeof(AccountRole),
					(await _userManager.GetRolesAsync(currentUser!)).FirstOrDefault()!
				);

			var lot = await _lotRepository.GetAsync(
				predicate: l => l.Id == request.LotId,
				cancellationToken: cancellationToken
			);

			var sellerId = currentUserRole == AccountRole.Seller ? currentUser!.Id : lot!.SellerId;
			var buyerId = currentUserRole == AccountRole.Buyer ? currentUser!.Id : lot!.BuyerId;

			#region If there is already a conversation between the buyer and the seller for this lot, return it

			var existingConversation = await _conversationRepository.GetAsync(
				predicate: c =>
					c.LotId == request.LotId && c.BuyerId == buyerId && c.SellerId == sellerId,
				cancellationToken: cancellationToken
			);

			if (existingConversation != null)
			{
				var existingConversationResponse = _mapper.Map<CreatedConversationResponse>(
					existingConversation
				);

				existingConversationResponse.Message = "Conversation already exists";

				return existingConversationResponse;
			}

			#endregion

			var conversation = new Conversation
			{
				BuyerId = (int)buyerId!, // because the BuyerId in Lot is nullable
				SellerId = sellerId,
				LotId = request.LotId
			};

			var result = await _conversationRepository.AddAsync(conversation);

			var createdConversationResponse = _mapper.Map<CreatedConversationResponse>(result);

			createdConversationResponse.Message = "Conversation created successfully";

			return createdConversationResponse;
		}
	}
}
