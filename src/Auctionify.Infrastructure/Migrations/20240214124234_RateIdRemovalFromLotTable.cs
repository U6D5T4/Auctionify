using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Auctionify.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RateIdRemovalFromLotTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RateId",
                table: "Lots");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RateId",
                table: "Lots",
                type: "int",
                nullable: true);
        }
    }
}
