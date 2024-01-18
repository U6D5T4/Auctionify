using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Features.Lots.Commands.UpdateLotStatus;
using Auctionify.Core.Entities;
using Auctionify.Core.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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

			var lot = await lotRepository.GetAsync(
				x => x.Id == lotId,
				include: x => x.Include(l => l.LotStatus).Include(l => l.Bids)
			);

			if (lot == null)
			{
				return;
			}

			Enum.TryParse(lot.LotStatus.Name, out AuctionStatus lotStatus);

			AuctionStatus futureStatus = AuctionStatus.NotSold;

			if (lot.Bids.Count > 0)
			{
				futureStatus = AuctionStatus.Sold;

				var highestBid = await bidRepository
					.Query()
					.Where(x => x.LotId == lotId && !x.BidRemoved)
					.OrderByDescending(x => x.NewPrice)
					.FirstOrDefaultAsync();

				if (highestBid != null)
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

					if (conversation == null)
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
