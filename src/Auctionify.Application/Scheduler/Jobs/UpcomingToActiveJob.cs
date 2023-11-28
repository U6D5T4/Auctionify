using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Features.Lots.Commands.UpdateLotStatus;
using Auctionify.Core.Enums;
using MediatR;
using Quartz;

namespace Auctionify.Application.Scheduler.Jobs
{
	public class UpcomingToActiveJob : IJob
	{
		private readonly IJobSchedulerService _jobSchedulerService;
		private readonly IMediator _mediator;
		private readonly ILotRepository _lotRepository;

		public UpcomingToActiveJob(IJobSchedulerService jobSchedulerService,
			IMediator mediator,
			ILotRepository lotRepository)
		{
			_jobSchedulerService = jobSchedulerService;
			_mediator = mediator;
			_lotRepository = lotRepository;
		}

		public async Task Execute(IJobExecutionContext context)
		{
			var lotId = context.MergedJobDataMap.GetInt("lotId");
			var lot = await _lotRepository.GetAsync(x => x.Id == lotId);

			if (lot == null) { return; }

			var result = await _mediator.Send(new UpdateLotStatusCommand { Id = lotId, Name = AuctionStatus.Active.ToString() });

			await _jobSchedulerService.ScheduleLotFinishJob(lotId, lot.EndDate);
		}
	}
}
