using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Auctionify.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RateAndLotTablesRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Rates_LotId",
                table: "Rates");

            migrationBuilder.CreateIndex(
                name: "IX_Rates_LotId",
                table: "Rates",
                column: "LotId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Rates_LotId",
                table: "Rates");

            migrationBuilder.CreateIndex(
                name: "IX_Rates_LotId",
                table: "Rates",
                column: "LotId",
                unique: true);
        }
    }
}
