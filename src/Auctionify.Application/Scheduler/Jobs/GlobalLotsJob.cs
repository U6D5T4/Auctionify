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
                            AuctionStatus futureStatus = AuctionStatus.NotSold;

                            if (lot.Bids.Count > 0)
                            {
                                futureStatus = AuctionStatus.Sold;

                                var highestBid = await bidRepository
                                    .Query()
                                    .Where(x => x.LotId == lot.Id && !x.BidRemoved)
                                    .OrderByDescending(x => x.NewPrice)
                                    .FirstOrDefaultAsync();

                                if (highestBid != null)
                                {
                                    lot.BuyerId = highestBid.BuyerId;
                                    lot.StartingPrice = highestBid.NewPrice;
                                    await lotRepository.UpdateAsync(lot);

                                    _logger.LogInformation(
                                        "FinishLot job updated lot with id: {lotId} with buyer id: {buyerId}",
                                        lot.Id,
                                        highestBid.BuyerId
                                    );
                                }
                            }

                            await mediator.Send( new UpdateLotStatusCommand { LotId = lot.Id, Name = futureStatus.ToString() });
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
                var jobKey = new JobKey(JobSchedulerService.FormDeleteDraftLotJobKey(lot.Id), JobSchedulerService.draftLotDeleteGroup);
                result = await scheduler.CheckExists(jobKey);
            }
            else if (lot.LotStatus.Name == AuctionStatus.Upcoming.ToString())
            {
                var jobKey = new JobKey(JobSchedulerService.FormUpcomingActiveJobKey(lot.Id), JobSchedulerService.upcomingActiveGroup);
                result = await scheduler.CheckExists(jobKey);
            }
            else if (lot.LotStatus.Name == AuctionStatus.Active.ToString())
            {
                var jobKey = new JobKey(JobSchedulerService.FormFinishLotJobKey(lot.Id), JobSchedulerService.finishGroup);
                result = await scheduler.CheckExists(jobKey);
            }

            return result;
        }
    }
}
