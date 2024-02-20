using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Auctionify.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGetReportDataProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			var procedure = @"CREATE OR ALTER PROCEDURE GetSellerReportData
                        @UserId INT,
                        @MonthsBack INT
                    AS
                    BEGIN
                        DECLARE @StartDate DATETIME = DATEADD(MONTH, -@MonthsBack, GETDATE());
    
                        DECLARE @TotalLots INT = (
                            SELECT ISNULL(COUNT(*), 0)
                            FROM Lots
                            WHERE SellerId = @UserId
                            AND Lots.CreationDate >= @StartDate
                        );
    
                        DECLARE @SoldLotsCount INT = (
                            SELECT ISNULL(COUNT(*), 0)
                            FROM Lots
                            INNER JOIN LotStatuses ON Lots.LotStatusId = LotStatuses.Id
                            WHERE SellerId = @UserId
                            AND LotStatuses.Name = 'Sold'
                            AND Lots.CreationDate >= @StartDate
                        );
    
                        DECLARE @TotalSoldAmount DECIMAL(18,2) = (
                            SELECT ISNULL(SUM(MaxBid), 0.00)
                            FROM (
                                SELECT MAX(NewPrice) AS MaxBid
                                FROM Bids
                                INNER JOIN Lots ON Bids.LotId = Lots.Id
                                INNER JOIN LotStatuses ON Lots.LotStatusId = LotStatuses.Id
                                WHERE Lots.SellerId = @UserId
                                AND LotStatuses.Name = 'Sold'
                                AND Lots.CreationDate >= @StartDate
                                GROUP BY Bids.LotId
                            ) AS SoldLotsMaxBids
                        );
    
                        SELECT @TotalLots AS TotalLots, @SoldLotsCount AS SoldLotsCount, @TotalSoldAmount AS TotalSoldAmount;
                    END;
                    GO";

			migrationBuilder.Sql(procedure);
		}

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetSellerReportData");
		}
    }
}
