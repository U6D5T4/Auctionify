using Auctionify.API.Services;
using Auctionify.Application;
using Auctionify.Application.Common.Interfaces;
using Auctionify.Infrastructure;
using Auctionify.Infrastructure.Persistence;

namespace Auctionify.API
{
	public class Program
	{
		public async static Task Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

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

                using (var scope = app.Services.CreateScope())
				{
                    var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitializer>();
                    await initialiser.InitialiseAsync();
                    await initialiser.SeedAsync();
                }
            }

			app.UseHttpsRedirection();
			app.UseAuthentication();
			app.UseAuthorization();
			app.MapControllers();
			app.MapRazorPages();
			app.Run();
		}
	}
}