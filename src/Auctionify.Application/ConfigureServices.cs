using Auctionify.Application.Common.Behaviours;
using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Scheduler;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using System.Reflection;

namespace Auctionify.Application
{
	public static class ConfigureServices
	{
		public static IServiceCollection AddApplicationServices(this IServiceCollection services)
		{
			services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
			services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
			services.AddScoped<IJobSchedulerService, JobSchedulerService>();

			services.AddAutoMapper(Assembly.GetExecutingAssembly());
			services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
			services.AddMediatR(config =>
			{
				config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
			});

			services.AddQuartz(q =>
			{
			});

			services.AddQuartzHostedService(opt =>
			{
				opt.WaitForJobsToComplete = false;
			});

			services.AddScoped<ApplicationExistingLotsScheduler>();

			ValidatorOptions.Global.LanguageManager.Culture = new System.Globalization.CultureInfo("en-US");

			return services;
		}
	}
}
