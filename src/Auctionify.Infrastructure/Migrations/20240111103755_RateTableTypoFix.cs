using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Auctionify.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RateTableTypoFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rates_Users_RecieverId",
                table: "Rates");

            migrationBuilder.RenameColumn(
                name: "RecieverId",
                table: "Rates",
                newName: "ReceiverId");

            migrationBuilder.RenameIndex(
                name: "IX_Rates_RecieverId",
                table: "Rates",
                newName: "IX_Rates_ReceiverId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rates_Users_ReceiverId",
                table: "Rates",
                column: "ReceiverId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rates_Users_ReceiverId",
                table: "Rates");

            migrationBuilder.RenameColumn(
                name: "ReceiverId",
                table: "Rates",
                newName: "RecieverId");

            migrationBuilder.RenameIndex(
                name: "IX_Rates_ReceiverId",
                table: "Rates",
                newName: "IX_Rates_RecieverId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rates_Users_RecieverId",
                table: "Rates",
                column: "RecieverId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
