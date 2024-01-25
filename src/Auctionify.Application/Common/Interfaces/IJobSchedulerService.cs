namespace Auctionify.Application.Common.Interfaces
{
	public interface IJobSchedulerService
	{
		Task ScheduleUpcomingToActiveLotStatusJob(int lotId, DateTime startDate);

		Task RemoveUpcomingToActiveJob(int lotId);

		Task ScheduleLotFinishJob(int lotId, DateTime endDate);

		Task RemoveLotFinishJob(int lotId);

		Task ScheduleDraftLotDeleteJob(int lotId, DateTime deleteDateTime);
	}
}
