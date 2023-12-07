using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Features.Lots.Commands.UpdateLotStatus;
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

		public FinishLotJob(IServiceScopeFactory scopeFactory,
			ILogger<FinishLotJob> logger)
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
			var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();


			var lot = await lotRepository.GetAsync(x => x.Id == lotId,
				include: x =>
							x.Include(l => l.LotStatus)
							.Include(l => l.Bids));
			
			if (lot == null) { return; }

			Enum.TryParse(lot.LotStatus.Name, out AuctionStatus lotStatus);

			AuctionStatus futureStatus = AuctionStatus.NotSold;

			if (lot.Bids.Count > 0)
			{
				futureStatus = AuctionStatus.Sold;
			}

			var result = await mediator.Send(new UpdateLotStatusCommand { Id = lotId, Name = futureStatus.ToString() });

			if (result != null)
			{
				_logger.LogInformation("FinishLot job finished working for lot with id: {lotId}", lotId);
			}
		}
	}
}
