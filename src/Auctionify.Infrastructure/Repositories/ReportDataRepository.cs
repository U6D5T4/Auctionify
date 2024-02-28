using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Interfaces.Repositories;
using Auctionify.Application.Common.Models.Report;
using Auctionify.Core.Entities;
using Auctionify.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Threading;

namespace Auctionify.Infrastructure.Repositories
{
	public class ReportDataRepository : IReportDataRepository
	{
		private readonly ApplicationDbContext _context;
		public ReportDataRepository(
			ApplicationDbContext context
		)
		{
			_context = context;

		}

		public async Task<ReportData> GetReportDataAsync(ReportRequest request)
		{
			var connectionString = _context.Database.GetConnectionString();
			await using var connection = new SqlConnection(connectionString);
			await connection.OpenAsync();

			await using var command = connection.CreateCommand();
			command.CommandType = CommandType.StoredProcedure;
			command.CommandText = "GetSellerReportData";
			command.Parameters.Add(new SqlParameter("@UserId", request.UserId));
			command.Parameters.Add(new SqlParameter("@MonthsBack", request.MonthsDuration));

			var reportData = new ReportData();
			await using var reader = await command.ExecuteReaderAsync();

			if (await reader.ReadAsync())
			{
				reportData.TotalSoldItems = reader.GetInt32(0);
				reportData.TotalCostOfSoldItems = reader.GetDecimal(1);
			}

			if (await reader.NextResultAsync())
			{
				while (await reader.ReadAsync())
				{
					var monthlyData = new MonthlyReportData
					{
						ReportMonth = reader.GetString(0),
						SoldLotsCount = reader.GetInt32(1),
						SoldLotsTotalCost = reader.GetDecimal(2),
						CreatedLotsCount = reader.GetInt32(3)
					};
					reportData.MonthlyReports.Add(monthlyData);
				}
			}

			return reportData;
		}
	}
}
