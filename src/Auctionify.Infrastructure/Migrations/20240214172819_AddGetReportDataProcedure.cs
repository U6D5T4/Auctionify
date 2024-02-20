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
    
                        -- Total count of all lots listed by the user
                        DECLARE @TotalLots INT = (
                            SELECT COUNT(*)
                            FROM Lots
                            WHERE SellerId = @UserId
                            AND Lots.CreationDate >= @StartDate -- Specify the table name before CreationDate
                        );
    
                        -- Count of lots that have been sold
                        DECLARE @SoldLotsCount INT = (
                            SELECT COUNT(*)
                            FROM Lots
                            INNER JOIN LotStatuses ON Lots.LotStatusId = LotStatuses.Id
                            WHERE SellerId = @UserId
                            AND LotStatuses.Name = 'Sold'
                            AND Lots.CreationDate >= @StartDate -- Specify the table name before CreationDate
                        );
    
                        -- Total sum of the highest bids for the sold lots
                        DECLARE @TotalSoldAmount DECIMAL(18,2) = (
                            SELECT SUM(MaxBid)
                            FROM (
                                SELECT MAX(NewPrice) AS MaxBid -- Replace 'Amount' with the actual column name for bid amount
                                FROM Bids
                                INNER JOIN Lots ON Bids.LotId = Lots.Id
                                INNER JOIN LotStatuses ON Lots.LotStatusId = LotStatuses.Id
                                WHERE Lots.SellerId = @UserId
                                AND LotStatuses.Name = 'Sold'
                                AND Lots.CreationDate >= @StartDate -- Specify the table name before CreationDate
                                GROUP BY Bids.LotId
                            ) AS SoldLotsMaxBids
                        );
    
                        -- Return the results
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
