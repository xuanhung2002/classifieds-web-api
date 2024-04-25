using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Classifieds.Data.Migrations
{
    /// <inheritdoc />
    public partial class u5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Auctions");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Bids",
                newName: "PostId");

            migrationBuilder.RenameColumn(
                name: "AuctionId",
                table: "Bids",
                newName: "BidderId");

            migrationBuilder.AddColumn<int>(
                name: "AuctionStatus",
                table: "Posts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CurrentAmount",
                table: "Posts",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CurrentBidderId",
                table: "Posts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndTime",
                table: "Posts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "StartAmount",
                table: "Posts",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuctionStatus",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "CurrentAmount",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "CurrentBidderId",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "StartAmount",
                table: "Posts");

            migrationBuilder.RenameColumn(
                name: "PostId",
                table: "Bids",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "BidderId",
                table: "Bids",
                newName: "AuctionId");

            migrationBuilder.CreateTable(
                name: "Auctions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CurrentAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CurrentBidderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auctions", x => x.Id);
                });
        }
    }
}
