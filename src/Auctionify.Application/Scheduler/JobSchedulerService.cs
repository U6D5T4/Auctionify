using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Scheduler.Jobs;
using Quartz;
using Quartz.Spi;

namespace Auctionify.Application.Scheduler
{
	public class JobSchedulerService : IJobSchedulerService
	{
		public static readonly string lotIdJobDataParam = "lotId";

		private readonly ISchedulerFactory _schedulerFactory;
		private readonly IJobFactory _customJobFactory;

		public static readonly string finishGroup = "finish";
		public static readonly string upcomingActiveGroup = "upcoming-active";
		public static readonly string draftLotDeleteGroup = "draft-delete";

		public JobSchedulerService(ISchedulerFactory schedulerFactory, IJobFactory customJobFactory)
		{
			_schedulerFactory = schedulerFactory;
			_customJobFactory = customJobFactory;
		}

		public async Task RemoveLotFinishJob(int lotId)
		{
			var scheduler = await _schedulerFactory.GetScheduler();
			scheduler.JobFactory = _customJobFactory;
			var jobKey = new JobKey(FormJobKey(finishGroup, lotId), finishGroup);
			await scheduler.DeleteJob(jobKey);
		}

		public async Task RemoveUpcomingToActiveJob(int lotId)
		{
			var scheduler = await _schedulerFactory.GetScheduler();
			scheduler.JobFactory = _customJobFactory;
			var jobKey = new JobKey(FormJobKey(upcomingActiveGroup, lotId), upcomingActiveGroup);
			await scheduler.DeleteJob(jobKey);
		}

		public async Task ScheduleLotFinishJob(int lotId, DateTime endDate)
		{
			var scheduler = await _schedulerFactory.GetScheduler();
			scheduler.JobFactory = _customJobFactory;

			var job = JobBuilder
				.Create<FinishLotJob>()
				.WithIdentity(FormJobKey(finishGroup, lotId), finishGroup)
				.UsingJobData(lotIdJobDataParam, lotId)
				.Build();

			var trigger = TriggerBuilder
				.Create()
				.WithIdentity(FormJobKey(finishGroup, lotId), finishGroup)
				.StartAt(endDate)
				.Build();

			await scheduler.ScheduleJob(job, trigger);
		}

		public async Task ScheduleUpcomingToActiveLotStatusJob(int lotId, DateTime startDate)
		{
			var scheduler = await _schedulerFactory.GetScheduler();
			scheduler.JobFactory = _customJobFactory;

			var job = JobBuilder
				.Create<UpcomingToActiveJob>()
				.WithIdentity(FormJobKey(upcomingActiveGroup, lotId), upcomingActiveGroup)
				.UsingJobData(lotIdJobDataParam, lotId)
				.Build();

			var trigger = TriggerBuilder
				.Create()
				.WithIdentity(FormJobKey(upcomingActiveGroup, lotId), upcomingActiveGroup)
				.StartAt(startDate)
				.Build();

			await scheduler.ScheduleJob(job, trigger);
		}

		public async Task ScheduleDraftLotDeleteJob(int lotId, DateTime deleteDateTime)
		{
			var scheduler = await _schedulerFactory.GetScheduler();
			scheduler.JobFactory = _customJobFactory;

			var job = JobBuilder
				.Create<DraftLotDeleteJob>()
				.WithIdentity(FormJobKey(draftLotDeleteGroup, lotId), draftLotDeleteGroup)
				.UsingJobData(lotIdJobDataParam, lotId)
				.Build();

			var trigger = TriggerBuilder
				.Create()
				.WithIdentity(FormJobKey(draftLotDeleteGroup, lotId), draftLotDeleteGroup)
				.StartAt(deleteDateTime)
				.Build();

			await scheduler.ScheduleJob(job, trigger);
		}

		public async Task ScheduleGlobalLotsJob()
		{
			var scheduler = await _schedulerFactory.GetScheduler();
			scheduler.JobFactory = _customJobFactory;

			var job = JobBuilder.Create<GlobalLotsJob>().WithIdentity("GlobalLotsJob").Build();

			var trigger = TriggerBuilder
				.Create()
				.WithIdentity("GlobalLotsJobTrigger")
				.StartNow()
				.WithSimpleSchedule(x => x.WithIntervalInMinutes(5))
				.Build();

			await scheduler.ScheduleJob(job, trigger);
		}

		public static string FormJobKey(string groupName, int lotId)
		{
			return $"{groupName}-{lotId}";
		}
	}
}
