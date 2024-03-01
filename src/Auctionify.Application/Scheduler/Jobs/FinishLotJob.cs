using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Common.Options;
using Auctionify.Application.Features.Lots.Commands.UpdateLotStatus;
using Auctionify.Application.Hubs;
using Auctionify.Core.Entities;
using Auctionify.Core.Enums;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;

namespace Auctionify.Application.Scheduler.Jobs
{
	public class FinishLotJob : IJob
	{
		private readonly IServiceScopeFactory _scopeFactory;
		private readonly ILogger<FinishLotJob> _logger;

		public FinishLotJob(IServiceScopeFactory scopeFactory, ILogger<FinishLotJob> logger)
		{
			_scopeFactory = scopeFactory;
			_logger = logger;
		}

		public async Task Execute(IJobExecutionContext context)
		{
			var lotId = context.MergedJobDataMap.GetInt(JobSchedulerService.lotIdJobDataParam);
			_logger.LogInformation("FinishLot job started working for lot with id: {lotId}", lotId);

			using var scope = _scopeFactory.CreateScope();
			var lotRepository = scope.ServiceProvider.GetRequiredService<ILotRepository>();
			var bidRepository = scope.ServiceProvider.GetRequiredService<IBidRepository>();
			var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
			var conversationRepository =
				scope.ServiceProvider.GetRequiredService<IConversationRepository>();
			var chatMessageRepository =
				scope.ServiceProvider.GetRequiredService<IChatMessageRepository>();
			var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<AuctionHub>>();

			var appUrlOptions = scope
				.ServiceProvider.GetRequiredService<IOptions<AppUrlOptions>>()
				.Value;

			var lot = await lotRepository.GetAsync(
				x => x.Id == lotId,
				include: x =>
					x.Include(l => l.Currency)
						.Include(l => l.LotStatus)
						.Include(l => l.Bids)
						.Include(x => x.Seller)
			);

			if (lot == null)
			{
				return;
			}

			_ = Enum.TryParse(lot.LotStatus.Name, out AuctionStatus lotStatus);

			AuctionStatus futureStatus = AuctionStatus.NotSold;

			if (lot.Bids.Count > 0)
			{
				futureStatus = AuctionStatus.Sold;

				var highestBid = await bidRepository
					.Query()
					.Where(x => x.LotId == lotId && !x.BidRemoved)
					.OrderByDescending(x => x.NewPrice)
					.FirstOrDefaultAsync();

				if (highestBid is not null)
				{
					lot.BuyerId = highestBid.BuyerId;
					lot.StartingPrice = highestBid.NewPrice;
					await lotRepository.UpdateAsync(lot);

					var conversation = await conversationRepository.GetAsync(
						x =>
							x.LotId == lotId
							&& x.BuyerId == highestBid.BuyerId
							&& x.SellerId == lot.SellerId
					);

					#region Creating a conversation after the lot is sold and there is a buyer

					if (conversation is null)
					{
						conversation = new Conversation
						{
							LotId = lotId,
							BuyerId = highestBid.BuyerId,
							SellerId = lot.SellerId
						};

						await conversationRepository.AddAsync(conversation);
					}

					#endregion

					#region Creating a congratulation message for the buyer

					var congratulationMessage = new ChatMessage
					{
						SenderId = lot.SellerId,
						ConversationId = conversation.Id,
						Body =
							$"Congratulations, Dear Buyer! "
							+ $"My name is {lot.Seller.FirstName} {lot.Seller.LastName} and I am the seller of the lot: \"{lot.Title}\". "
							+ $"I am happy to inform you that you won this auction with a bid of {highestBid.NewPrice} {lot.Currency.Code}! "
							+ $"Access the lot details through link: {appUrlOptions.ClientApp}/get-lot/{lotId}",
						IsRead = false,
					};

					await chatMessageRepository.AddAsync(congratulationMessage);

					await hubContext
						.Clients.Group(conversation.Id.ToString())
						.SendAsync(
							SignalRActions.ReceiveChatMessageNotification,
							cancellationToken: context.CancellationToken
						);

					#endregion

					_logger.LogInformation(
						"FinishLot job updated lot with id: {lotId} with buyer id: {buyerId}",
						lotId,
						highestBid.BuyerId
					);
				}
			}

			var result = await mediator.Send(
				new UpdateLotStatusCommand { LotId = lotId, Name = futureStatus.ToString() }
			);

			if (result != null)
			{
				_logger.LogInformation(
					"FinishLot job finished working for lot with id: {lotId}",
					lotId
				);
			}
		}
	}
}
