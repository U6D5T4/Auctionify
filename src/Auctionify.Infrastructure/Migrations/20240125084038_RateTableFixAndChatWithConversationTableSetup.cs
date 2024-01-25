using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Auctionify.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RateTableFixAndChatWithConversationTableSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_Lots_LotId",
                table: "ChatMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_Users_BuyerId",
                table: "ChatMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_Users_SellerId",
                table: "ChatMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_Rates_Users_RecieverId",
                table: "Rates");

            migrationBuilder.DropIndex(
                name: "IX_ChatMessages_BuyerId",
                table: "ChatMessages");

            migrationBuilder.DropColumn(
                name: "BuyerId",
                table: "ChatMessages");

            migrationBuilder.RenameColumn(
                name: "RecieverId",
                table: "Rates",
                newName: "ReceiverId");

            migrationBuilder.RenameIndex(
                name: "IX_Rates_RecieverId",
                table: "Rates",
                newName: "IX_Rates_ReceiverId");

            migrationBuilder.RenameColumn(
                name: "SellerId",
                table: "ChatMessages",
                newName: "SenderId");

            migrationBuilder.RenameColumn(
                name: "Message",
                table: "ChatMessages",
                newName: "Body");

            migrationBuilder.RenameColumn(
                name: "LotId",
                table: "ChatMessages",
                newName: "ConversationId");

            migrationBuilder.RenameIndex(
                name: "IX_ChatMessages_SellerId",
                table: "ChatMessages",
                newName: "IX_ChatMessages_SenderId");

            migrationBuilder.RenameIndex(
                name: "IX_ChatMessages_LotId",
                table: "ChatMessages",
                newName: "IX_ChatMessages_ConversationId");

            migrationBuilder.AddColumn<bool>(
                name: "IsRead",
                table: "ChatMessages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Conversations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BuyerId = table.Column<int>(type: "int", nullable: false),
                    SellerId = table.Column<int>(type: "int", nullable: false),
                    LotId = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conversations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Conversations_Lots_LotId",
                        column: x => x.LotId,
                        principalTable: "Lots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Conversations_Users_BuyerId",
                        column: x => x.BuyerId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Conversations_Users_SellerId",
                        column: x => x.SellerId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_BuyerId",
                table: "Conversations",
                column: "BuyerId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_LotId",
                table: "Conversations",
                column: "LotId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_SellerId",
                table: "Conversations",
                column: "SellerId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_Conversations_ConversationId",
                table: "ChatMessages",
                column: "ConversationId",
                principalTable: "Conversations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_Users_SenderId",
                table: "ChatMessages",
                column: "SenderId",
                principalTable: "Users",
                principalColumn: "Id");

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
                name: "FK_ChatMessages_Conversations_ConversationId",
                table: "ChatMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_Users_SenderId",
                table: "ChatMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_Rates_Users_ReceiverId",
                table: "Rates");

            migrationBuilder.DropTable(
                name: "Conversations");

            migrationBuilder.DropColumn(
                name: "IsRead",
                table: "ChatMessages");

            migrationBuilder.RenameColumn(
                name: "ReceiverId",
                table: "Rates",
                newName: "RecieverId");

            migrationBuilder.RenameIndex(
                name: "IX_Rates_ReceiverId",
                table: "Rates",
                newName: "IX_Rates_RecieverId");

            migrationBuilder.RenameColumn(
                name: "SenderId",
                table: "ChatMessages",
                newName: "SellerId");

            migrationBuilder.RenameColumn(
                name: "ConversationId",
                table: "ChatMessages",
                newName: "LotId");

            migrationBuilder.RenameColumn(
                name: "Body",
                table: "ChatMessages",
                newName: "Message");

            migrationBuilder.RenameIndex(
                name: "IX_ChatMessages_SenderId",
                table: "ChatMessages",
                newName: "IX_ChatMessages_SellerId");

            migrationBuilder.RenameIndex(
                name: "IX_ChatMessages_ConversationId",
                table: "ChatMessages",
                newName: "IX_ChatMessages_LotId");

            migrationBuilder.AddColumn<int>(
                name: "BuyerId",
                table: "ChatMessages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_BuyerId",
                table: "ChatMessages",
                column: "BuyerId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_Lots_LotId",
                table: "ChatMessages",
                column: "LotId",
                principalTable: "Lots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_Users_BuyerId",
                table: "ChatMessages",
                column: "BuyerId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_Users_SellerId",
                table: "ChatMessages",
                column: "SellerId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Rates_Users_RecieverId",
                table: "Rates",
                column: "RecieverId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
