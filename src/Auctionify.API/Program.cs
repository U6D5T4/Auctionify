using Auctionify.API.Middlewares;
using Auctionify.API.Services;
using Auctionify.Application;
using Auctionify.Application.Common.Interfaces;
using Auctionify.Infrastructure;
using Auctionify.Infrastructure.Persistence;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Web;
using System.Text.Json.Serialization;

namespace Auctionify.API
{
	public class Program
	{
		public async static Task Main(string[] args)
		{
			var logger = NLog.LogManager.Setup()
			.LoadConfigurationFromAppSettings()
			.GetCurrentClassLogger();

			try
			{
				logger.Debug("init main");
				var builder = WebApplication.CreateBuilder(args);

				builder.Services.AddApplicationServices();
				builder.Services.AddInfrastructureServices(builder.Configuration);
				// Add services to the container.
				builder.Services.AddControllers()
								.AddJsonOptions(options =>
												options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
				// So that the URLs are lowercase
				builder.Services.AddRouting(options => options.LowercaseUrls = true);

				builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

				// Add services to the container.
				builder.Services.AddRazorPages();
				// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
				builder.Services.AddEndpointsApiExplorer();
				builder.Services.AddSwaggerGen(c =>
				{
					c.SwaggerDoc("v1", new OpenApiInfo
					{
						Title = "Auctionify",
						Version = "v1",
						Description = "API for Auctionify to manage auctions and bids for sellers and buyers.",
					});

                    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                    {
                        Name = "Authorization",
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
                    });

                    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme {
                                Reference = new OpenApiReference {
                                    Type = ReferenceType.SecurityScheme,
                                        Id = "Bearer"
                                }
                            },
                            new string[] {}
                        }
                    });
                });

				// To display enum values as strings in the response
				builder.Services
					.AddControllers()
					.AddJsonOptions(options =>
						options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));


				// NLog: Setup NLog for Dependency injection
				builder.Logging.ClearProviders();
				builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
				builder.Host.UseNLog();

				var app = builder.Build();

				// Configure the HTTP request pipeline.
				if (app.Environment.IsDevelopment())
				{
					app.UseSwagger();
					app.UseSwaggerUI();

					using var scope = app.Services.CreateScope();
					var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitializer>();
					await initialiser.InitialiseAsync();
					await initialiser.SeedAsync();
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
				app.UseCustomExceptionHandler(); // Custom exception handler middleware
				app.MapControllers();
				app.MapRazorPages();
				app.Run();
			}
            catch (HostAbortedException ex)
            {
                logger.Info("Ignore HostAbortedException", ex.Message);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Stopped program because of exception.");
                throw;
            }
            finally
			{
				NLog.LogManager.Shutdown();
			}
		}
	}
}