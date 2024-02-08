using Auctionify.Application.Common.Interfaces.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Auctionify.Application.Scheduler.Jobs
{
    public class DraftLotDeleteJob : IJob
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<DraftLotDeleteJob> _logger;

        public DraftLotDeleteJob(IServiceScopeFactory scopeFactory, ILogger<DraftLotDeleteJob> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var lotId = context.MergedJobDataMap.GetInt(JobSchedulerService.lotIdJobDataParam);
            _logger.LogInformation("DraftLotRemoval job started working for lot with id: {lotId}", lotId);

            using var scope = _scopeFactory.CreateScope();
            var lotRepository = scope.ServiceProvider.GetRequiredService<ILotRepository>();

            var lot = lotRepository.Query().FirstOrDefault(x  => x.Id == lotId);

            if (lot != null)
            {
                await lotRepository.DeleteAsync(lot);
            }
            else
                return;

            _logger.LogInformation(
                    "DraftLotRemoval job finished working for lot with id: {lotId}",
                    lotId
                );
        }
    }
}
