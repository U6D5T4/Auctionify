using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Scheduler.Jobs;
using Quartz;

namespace Auctionify.Application.Scheduler
{
	public class JobSchedulerService : IJobSchedulerService
	{
		private readonly ISchedulerFactory _schedulerFactory;

		public JobSchedulerService(ISchedulerFactory schedulerFactory)
		{
			_schedulerFactory = schedulerFactory;
		}

		public async Task RemoveLotFinishJob(int lotId)
		{
			var scheduler = await _schedulerFactory.GetScheduler();
			var jobKey = new JobKey($"finish-{lotId}", "finish");
			await scheduler.DeleteJob(jobKey);
		}

		public async Task RemoveUpcomingToActiveJob(int lotId)
		{
			var scheduler = await _schedulerFactory.GetScheduler();
			var jobKey = new JobKey($"upcoming-active-{lotId}", "upcoming-active");
			await scheduler.DeleteJob(jobKey);
		}

		public async Task ScheduleLotFinishJob(int lotId, DateTime endDate)
		{
			var scheduler = await _schedulerFactory.GetScheduler();

			var job = JobBuilder.Create<FinishLotJob>()
				.WithIdentity($"finish-{lotId}", "finish")
				.UsingJobData("lotId", lotId)
				.Build();

			var trigger = TriggerBuilder.Create()
				.WithIdentity($"finish-{lotId}", "finish")
				.StartAt(endDate)
				.Build();

			await scheduler.ScheduleJob(job, trigger);
		}

		public async Task ScheduleUpcomingToActiveLotStatusJob(int lotId, DateTime startDate)
		{
			var scheduler = await _schedulerFactory.GetScheduler();

			var job = JobBuilder.Create<UpcomingToActiveJob>()
				.WithIdentity($"upcoming-active-{lotId}", "upcoming-active")
				.UsingJobData("lotId", lotId)
				.Build();

			var trigger = TriggerBuilder.Create()
				.WithIdentity($"upcoming-active-{lotId}", "upcoming-active")
				.StartAt(startDate)
				.Build();

			await scheduler.ScheduleJob(job, trigger);
		}
	}
}
