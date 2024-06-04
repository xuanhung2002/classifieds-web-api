using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Classifieds.Data.Migrations
{
    /// <inheritdoc />
    public partial class okla3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatUsers_Chats_ChatId1",
                table: "ChatUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatUsers_Users_UserId1",
                table: "ChatUsers");

            migrationBuilder.DropIndex(
                name: "IX_ChatUsers_ChatId1",
                table: "ChatUsers");

            migrationBuilder.DropIndex(
                name: "IX_ChatUsers_UserId1",
                table: "ChatUsers");

            migrationBuilder.DropColumn(
                name: "ChatId1",
                table: "ChatUsers");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "ChatUsers");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "ChatUsers",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<Guid>(
                name: "ChatId",
                table: "ChatUsers",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_ChatUsers_ChatId",
                table: "ChatUsers",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatUsers_UserId",
                table: "ChatUsers",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatUsers_Chats_ChatId",
                table: "ChatUsers",
                column: "ChatId",
                principalTable: "Chats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatUsers_Users_UserId",
                table: "ChatUsers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatUsers_Chats_ChatId",
                table: "ChatUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatUsers_Users_UserId",
                table: "ChatUsers");

            migrationBuilder.DropIndex(
                name: "IX_ChatUsers_ChatId",
                table: "ChatUsers");

            migrationBuilder.DropIndex(
                name: "IX_ChatUsers_UserId",
                table: "ChatUsers");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "ChatUsers",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<int>(
                name: "ChatId",
                table: "ChatUsers",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "ChatId1",
                table: "ChatUsers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "ChatUsers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ChatUsers_ChatId1",
                table: "ChatUsers",
                column: "ChatId1");

            migrationBuilder.CreateIndex(
                name: "IX_ChatUsers_UserId1",
                table: "ChatUsers",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatUsers_Chats_ChatId1",
                table: "ChatUsers",
                column: "ChatId1",
                principalTable: "Chats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatUsers_Users_UserId1",
                table: "ChatUsers",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
