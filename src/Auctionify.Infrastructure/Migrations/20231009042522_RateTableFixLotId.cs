using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Auctionify.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RateTableFixLotId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lots_Rates_RateId",
                table: "Lots");

            migrationBuilder.DropIndex(
                name: "IX_Lots_RateId",
                table: "Lots");

            migrationBuilder.RenameColumn(
                name: "LoId",
                table: "Rates",
                newName: "LotId");

            migrationBuilder.CreateIndex(
                name: "IX_Rates_LotId",
                table: "Rates",
                column: "LotId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Rates_Lots_LotId",
                table: "Rates",
                column: "LotId",
                principalTable: "Lots",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rates_Lots_LotId",
                table: "Rates");

            migrationBuilder.DropIndex(
                name: "IX_Rates_LotId",
                table: "Rates");

            migrationBuilder.RenameColumn(
                name: "LotId",
                table: "Rates",
                newName: "LoId");

            migrationBuilder.CreateIndex(
                name: "IX_Lots_RateId",
                table: "Lots",
                column: "RateId",
                unique: true,
                filter: "[RateId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Lots_Rates_RateId",
                table: "Lots",
                column: "RateId",
                principalTable: "Rates",
                principalColumn: "Id");
        }
    }
}
