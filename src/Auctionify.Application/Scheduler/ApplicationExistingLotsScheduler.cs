using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Scheduler.Jobs;
using Auctionify.Core.Entities;
using Auctionify.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace Auctionify.Application.Scheduler
{
	public class ApplicationExistingLotsScheduler
	{
		private readonly ILotRepository _lotRepository;
		private readonly ISchedulerFactory _schedulerFactory;
		private readonly IJobSchedulerService _jobSchedulerService;

		public ApplicationExistingLotsScheduler(ILotRepository lotRepository, ISchedulerFactory schedulerFactory,
			IJobSchedulerService jobSchedulerService)
		{
			_lotRepository = lotRepository;
			_schedulerFactory = schedulerFactory;
			_jobSchedulerService = jobSchedulerService;
		}

		public async Task InitializeJobs()
		{
			var lots = await _lotRepository.Query().Include(x => x.LotStatus).ToListAsync();
			var scheduler = await _schedulerFactory.GetScheduler();

			foreach (var lot in lots)
			{
				Enum.TryParse(lot.LotStatus.Name, out AuctionStatus lotStatus);

				if (lotStatus == AuctionStatus.Upcoming)
				{
					if (lot.StartDate > DateTime.UtcNow)
					{
						await _jobSchedulerService.ScheduleUpcomingToActiveLotStatusJob(lot.Id, lot.StartDate);
					}
				}
			}

			var currentJobs = scheduler.GetJobGroupNames();
		}
	}
}
