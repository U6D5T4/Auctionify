using Auctionify.Application;
using Auctionify.Infrastructure;
using NLog;
using NLog.Web;

namespace Auctionify.API
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var logger = NLog.LogManager.Setup()
			.LoadConfigurationFromAppSettings()
			.GetCurrentClassLogger();

			try
			{
				var builder = WebApplication.CreateBuilder(args);

				builder.Services.AddApplicationServices();
				builder.Services.AddInfrastructureServices(builder.Configuration);

				// Add services to the container.
				builder.Services.AddControllers();
				builder.Services.AddRazorPages();
				// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
				builder.Services.AddEndpointsApiExplorer();
				builder.Services.AddSwaggerGen();

				var app = builder.Build();

				// Configure the HTTP request pipeline.
				if (app.Environment.IsDevelopment())
				{
					app.UseSwagger();
					app.UseSwaggerUI();
				}

				// So that the Swagger UI is available in production
				// To invoke the Swagger UI, go to https://<host>/swagger or https://<host>/swagger/index.html
				if (app.Environment.IsProduction())
				{
					app.UseSwagger();
					app.UseSwaggerUI(c =>
					{
						c.SwaggerEndpoint("/swagger/v1/swagger.json", "Auctionify v1");
						c.RoutePrefix = "swagger";
					});
				}

				app.UseRouting();
				app.UseHttpsRedirection();
				app.UseAuthentication();
				app.UseAuthorization();
				app.MapControllers();
				app.MapRazorPages();
				app.Run();
			}
			catch (Exception ex)
			{
				logger.Error(ex, "Stopped program because of exception");
				throw;
			}
			finally
			{
				NLog.LogManager.Shutdown();
			}
		}
	}
}