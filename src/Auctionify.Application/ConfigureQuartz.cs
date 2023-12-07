using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Scheduler.Jobs;
using Auctionify.Application.Scheduler;
using Microsoft.Extensions.DependencyInjection;
using Quartz.Impl;
using Quartz.Spi;
using Quartz;

namespace Auctionify.Application
{
	public static class ConfigureQuartz
	{
		public static IServiceCollection AddQuartzService(this IServiceCollection services)
		{
			services.AddSingleton<IJobFactory, CustomJobFactory>();
			services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
			services.AddSingleton<IJobSchedulerService, JobSchedulerService>();
			services.AddSingleton<UpcomingToActiveJob>();
			services.AddSingleton<FinishLotJob>();

			services.AddQuartz(q =>
			{
				q.UseJobFactory<CustomJobFactory>();
			});

			services.AddQuartzHostedService(opt =>
			{
				opt.WaitForJobsToComplete = false;
			});

			services.AddScoped<ApplicationExistingLotsScheduler>();

			services.AddHostedService<ApplicationExistingLotsScheduler>();

			return services;
		}
	}
}
