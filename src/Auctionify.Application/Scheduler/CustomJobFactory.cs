using Quartz;
using Quartz.Spi;

namespace Auctionify.Application.Scheduler
{
	public class CustomJobFactory : IJobFactory
	{
		private readonly IServiceProvider _serviceProvider;
		public CustomJobFactory(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}
		public IJob NewJob(TriggerFiredBundle bundle,
			IScheduler scheduler)
		{
			var jobDetail = bundle.JobDetail;
			return (IJob) _serviceProvider.GetService(jobDetail.JobType);
		}
		public void ReturnJob(IJob job)
		{
			var disposable = job as IDisposable;
			disposable?.Dispose();
		}
	}
}
