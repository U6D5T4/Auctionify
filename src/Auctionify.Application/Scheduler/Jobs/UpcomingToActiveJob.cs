using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Features.Lots.Commands.UpdateLotStatus;
using Auctionify.Core.Enums;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Auctionify.Application.Scheduler.Jobs
{
	public class UpcomingToActiveJob : IJob
	{
		private readonly IJobSchedulerService _jobSchedulerService;
		private readonly IServiceScopeFactory _scopeFactory;
		private readonly ILogger<UpcomingToActiveJob> _logger;

		public UpcomingToActiveJob(IJobSchedulerService jobSchedulerService,
			IServiceScopeFactory scopeFactory,
			ILogger<UpcomingToActiveJob> logger)
		{
			_jobSchedulerService = jobSchedulerService;
			_scopeFactory = scopeFactory;
			_logger = logger;
		}

		public async Task Execute(IJobExecutionContext context)
		{
			var lotId = context.MergedJobDataMap.GetInt(JobSchedulerService.lotIdJobDataParam);
			_logger.LogInformation("Upcoming-To-Active job started working for lot with id: {lotId}", lotId);

			using var scope = _scopeFactory.CreateScope();
			var lotRepository = scope.ServiceProvider.GetRequiredService<ILotRepository>();
			var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

			var lot = await lotRepository.GetAsync(x => x.Id == lotId);

			if (lot == null) { return; }

			var result = await mediator.Send(new UpdateLotStatusCommand { Id = lotId, Name = AuctionStatus.Active.ToString() });

			if (result != null)
			{
				_logger.LogInformation("Upcoming-To-Active job finished working for lot with id: {lotId}", lotId);
				await _jobSchedulerService.ScheduleLotFinishJob(lotId, lot.EndDate);
			}
		}
	}
}
