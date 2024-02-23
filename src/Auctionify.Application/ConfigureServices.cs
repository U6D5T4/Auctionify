using Auctionify.Application.Common.Behaviours;
using Auctionify.Application.Common.Options;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Auctionify.Application
{
	public static class ConfigureServices
	{
		public static IServiceCollection AddApplicationServices(
			this IServiceCollection services,
			IConfiguration configuration
		)
		{
			services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

			services.AddAutoMapper(Assembly.GetExecutingAssembly());
			services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
			services.AddMediatR(config =>
			{
				config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
			});

			services.Configure<AppUrlOptions>(configuration.GetSection(AppUrlOptions.App));

			ValidatorOptions.Global.LanguageManager.Culture = new System.Globalization.CultureInfo(
				"en-US"
			);

			return services;
		}
	}
}
