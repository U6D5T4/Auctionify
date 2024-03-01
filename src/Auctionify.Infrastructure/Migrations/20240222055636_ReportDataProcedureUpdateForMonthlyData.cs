using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Auctionify.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReportDataProcedureUpdateForMonthlyData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			var procedure = @"CREATE OR ALTER PROCEDURE GetSellerReportData
                    @UserId INT,
                    @MonthsBack INT
                AS
                BEGIN
                    DECLARE @CurrentDate DATE = GETDATE();
                    DECLARE @FirstMonthStart DATE = DATEADD(MONTH, -@MonthsBack, DATEADD(DAY, 1-DAY(GETDATE()), GETDATE()));
                    DECLARE @LastMonthEnd DATE = EOMONTH(@CurrentDate, -1);

                    CREATE TABLE #MonthlyData (
                        MonthYear VARCHAR(7),
                        SoldLotsCount INT,
                        TotalSoldAmount DECIMAL(18,2),
                        CreatedLotsCount INT
                    );

                    DECLARE @MonthIndex INT = 0;
                    WHILE @MonthIndex < @MonthsBack
                    BEGIN
                        DECLARE @MonthStart DATE = DATEADD(MONTH, @MonthIndex, @FirstMonthStart);
                        DECLARE @MonthEnd DATE = EOMONTH(@MonthStart);

                        INSERT INTO #MonthlyData (MonthYear, SoldLotsCount, TotalSoldAmount, CreatedLotsCount)
                        SELECT
                            FORMAT(@MonthStart, 'yyyy-MM') AS MonthYear,
                            COUNT(DISTINCT CASE WHEN LS.Name = 'Sold' AND L.EndDate BETWEEN @MonthStart AND @MonthEnd THEN L.Id END) AS SoldLotsCount,
                            ISNULL(SUM(CASE WHEN LS.Name = 'Sold' AND L.EndDate BETWEEN @MonthStart AND @MonthEnd THEN B.NewPrice ELSE 0 END), 0) AS TotalSoldAmount,
                            COUNT(DISTINCT CASE WHEN L.CreationDate BETWEEN @MonthStart AND @MonthEnd THEN L.Id END) AS CreatedLotsCount
                        FROM Lots L
                        LEFT JOIN LotStatuses LS ON L.LotStatusId = LS.Id
                        LEFT JOIN Bids B ON B.LotId = L.Id AND LS.Name = 'Sold'
                        WHERE L.SellerId = @UserId AND L.CreationDate >= @MonthStart AND L.CreationDate <= @MonthEnd

                        SET @MonthIndex = @MonthIndex + 1;
                    END

                    SELECT
                        ISNULL(SUM(SoldLotsCount), 0) AS TotalSoldItems,
                        ISNULL(SUM(TotalSoldAmount), 0) AS TotalCostOfSoldItems
                    FROM #MonthlyData
                    WHERE MonthYear BETWEEN FORMAT(@FirstMonthStart, 'yyyy-MM') AND FORMAT(@LastMonthEnd, 'yyyy-MM');

                    SELECT 
                        MonthYear,
                        SoldLotsCount,
                        TotalSoldAmount,
                        CreatedLotsCount
                    FROM #MonthlyData
                    WHERE MonthYear BETWEEN FORMAT(@FirstMonthStart, 'yyyy-MM') AND FORMAT(@LastMonthEnd, 'yyyy-MM')
                    ORDER BY MonthYear;

                    DROP TABLE #MonthlyData;
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
