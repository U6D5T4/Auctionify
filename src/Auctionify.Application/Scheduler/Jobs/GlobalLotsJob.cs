using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Features.Lots.Commands.UpdateLotStatus;
using Auctionify.Core.Entities;
using Auctionify.Core.Enums;
using Google.Apis.Logging;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Spi;

namespace Auctionify.Application.Scheduler.Jobs
{
    public class GlobalLotsJob : IJob
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<GlobalLotsJob> _logger;
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IJobFactory _customJobFactory;

        public GlobalLotsJob(IServiceScopeFactory scopeFactory,
            ILogger<GlobalLotsJob> logger,
            ISchedulerFactory schedulerFactory,
            IJobFactory customJobFactory)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _schedulerFactory = schedulerFactory;
            _customJobFactory = customJobFactory;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            using var scope = _scopeFactory.CreateScope();
            var lotRepository = scope.ServiceProvider.GetRequiredService<ILotRepository>();
            var lotStatusRepository = scope.ServiceProvider.GetRequiredService<ILotStatusRepository>();
            var bidRepository = scope.ServiceProvider.GetRequiredService<IBidRepository>();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var jobSchedulerService = scope.ServiceProvider.GetRequiredService<IJobSchedulerService>();

            var lots = await lotRepository.Query().Include(l => l.LotStatus).ToListAsync();
            var lotStatuses = await lotStatusRepository.Query().ToListAsync();

            foreach (var lot in lots)
            {
                if (!(await CheckIfLotHasJob(lot)))
                {
                    if (lot.LotStatus.Name == AuctionStatus.Draft.ToString())
                    {
                        var modDateDifferenece = DateTime.UtcNow - lot.ModificationDate;
                        if (modDateDifferenece.Days >= 7)
                        {
                            await lotRepository.DeleteAsync(lot);
                        }
                    }
                    else if (lot.LotStatus.Name == AuctionStatus.Upcoming.ToString())
                    {
                        if (lot.StartDate <= DateTime.UtcNow)
                        {
                            await mediator.Send(
                            new UpdateLotStatusCommand { LotId = lot.Id, Name = AuctionStatus.Active.ToString() });
                        }
                    }
                    else if (lot.LotStatus.Name == AuctionStatus.Active.ToString())
                    {
                        if (lot.EndDate <= DateTime.UtcNow)
                        {
                            await jobSchedulerService.ScheduleLotFinishJob(lot.Id, DateTime.UtcNow);
                        }
                    }
                }
            }
        }

        private async Task<bool> CheckIfLotHasJob(Lot lot)
        {
            var scheduler = await _schedulerFactory.GetScheduler();
            scheduler.JobFactory = _customJobFactory;
            bool result = false;

            if (lot.LotStatus.Name == AuctionStatus.Draft.ToString())
            {
                var jobKey = new JobKey(JobSchedulerService.FormJobKey(JobSchedulerService.draftLotDeleteGroup, lot.Id), JobSchedulerService.draftLotDeleteGroup);
                result = await scheduler.CheckExists(jobKey);
            }
            else if (lot.LotStatus.Name == AuctionStatus.Upcoming.ToString())
            {
                var jobKey = new JobKey(JobSchedulerService.FormJobKey(JobSchedulerService.upcomingActiveGroup, lot.Id), JobSchedulerService.upcomingActiveGroup);
                result = await scheduler.CheckExists(jobKey);
            }
            else if (lot.LotStatus.Name == AuctionStatus.Active.ToString())
            {
                var jobKey = new JobKey(JobSchedulerService.FormJobKey(JobSchedulerService.finishGroup, lot.Id), JobSchedulerService.finishGroup);
                result = await scheduler.CheckExists(jobKey);
            }

            return result;
        }
    }
}
