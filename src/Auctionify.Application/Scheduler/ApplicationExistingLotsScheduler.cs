using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;

namespace Auctionify.Application.Scheduler
{
	public class ApplicationExistingLotsScheduler : IHostedService
	{
		private readonly IJobSchedulerService _jobSchedulerService;
		private readonly IServiceScopeFactory _scopeFactory;

		public ApplicationExistingLotsScheduler(
			IJobSchedulerService jobSchedulerService,
			IServiceScopeFactory scopeFactory)
		{
			_jobSchedulerService = jobSchedulerService;
			_scopeFactory = scopeFactory;
		}

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			using var scope = _scopeFactory.CreateScope();
			var lotRepository = scope.ServiceProvider.GetRequiredService<ILotRepository>();

			var lots = await lotRepository.Query().Include(x => x.LotStatus).ToListAsync();

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
				else if (lotStatus == AuctionStatus.Active)
				{
					if (lot.EndDate > DateTime.UtcNow)
					{
						await _jobSchedulerService.ScheduleLotFinishJob(lot.Id, lot.EndDate);
					}
				}
				else if (lotStatus == AuctionStatus.Draft)
				{
					var deleteTime = lot.ModificationDate.AddDays(7);
					await _jobSchedulerService.ScheduleDraftLotDeleteJob(lot.Id, deleteTime);
				}
			}
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}
