using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Classifieds.Data.Migrations
{
    /// <inheritdoc />
    public partial class okla4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_ChatUsers_User_UserId",
            //    table: "ChatUsers");

            //migrationBuilder.DropUniqueConstraint(
            //    name: "AK_User_TempId1",
            //    table: "User");

            //migrationBuilder.RenameTable(
            //    name: "User",
            //    newName: "Users");

            //migrationBuilder.RenameColumn(
            //    name: "TempId1",
            //    table: "Users",
            //    newName: "Id");

            //migrationBuilder.AddColumn<string>(
            //    name: "AccountName",
            //    table: "Users",
            //    type: "nvarchar(max)",
            //    nullable: false,
            //    defaultValue: "");

            //migrationBuilder.AddColumn<string>(
            //    name: "Avatar",
            //    table: "Users",
            //    type: "nvarchar(max)",
            //    nullable: true);

            //migrationBuilder.AddColumn<DateTime>(
            //    name: "CreatedAt",
            //    table: "Users",
            //    type: "datetime2",
            //    nullable: false,
            //    defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            //migrationBuilder.AddColumn<string>(
            //    name: "Email",
            //    table: "Users",
            //    type: "nvarchar(max)",
            //    nullable: false,
            //    defaultValue: "");

            //migrationBuilder.AddColumn<string>(
            //    name: "Name",
            //    table: "Users",
            //    type: "nvarchar(max)",
            //    nullable: false,
            //    defaultValue: "");

            //migrationBuilder.AddColumn<byte[]>(
            //    name: "PasswordHash",
            //    table: "Users",
            //    type: "varbinary(max)",
            //    nullable: false,
            //    defaultValue: new byte[0]);

            //migrationBuilder.AddColumn<DateTime>(
            //    name: "PasswordReset",
            //    table: "Users",
            //    type: "datetime2",
            //    nullable: true);

            //migrationBuilder.AddColumn<byte[]>(
            //    name: "PasswordSalt",
            //    table: "Users",
            //    type: "varbinary(max)",
            //    nullable: false,
            //    defaultValue: new byte[0]);

            //migrationBuilder.AddColumn<string>(
            //    name: "PhoneNumber",
            //    table: "Users",
            //    type: "nvarchar(max)",
            //    nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "ResetToken",
            //    table: "Users",
            //    type: "nvarchar(max)",
            //    nullable: true);

            //migrationBuilder.AddColumn<DateTime>(
            //    name: "ResetTokenExpires",
            //    table: "Users",
            //    type: "datetime2",
            //    nullable: true);

            //migrationBuilder.AddColumn<int>(
            //    name: "Role",
            //    table: "Users",
            //    type: "int",
            //    nullable: false,
            //    defaultValue: 0);

            //migrationBuilder.AddColumn<DateTime>(
            //    name: "UpdatedAt",
            //    table: "Users",
            //    type: "datetime2",
            //    nullable: false,
            //    defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            //migrationBuilder.AddPrimaryKey(
            //    name: "PK_Users",
            //    table: "Users",
            //    column: "Id");

            //    migrationBuilder.CreateTable(
            //        name: "Categories",
            //        columns: table => new
            //        {
            //            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //            Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //            Image = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //            CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            //            UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_Categories", x => x.Id);
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "Notifications",
            //        columns: table => new
            //        {
            //            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //            Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //            UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //            Seen = table.Column<bool>(type: "bit", nullable: false),
            //            CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            //            UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_Notifications", x => x.Id);
            //            table.ForeignKey(
            //                name: "FK_Notifications_Users_UserId",
            //                column: x => x.UserId,
            //                principalTable: "Users",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Cascade);
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "Posts",
            //        columns: table => new
            //        {
            //            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //            Subject = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //            Images = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //            Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //            Price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
            //            ItemCondition = table.Column<int>(type: "int", nullable: false),
            //            Status = table.Column<int>(type: "int", nullable: false),
            //            AddressJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //            UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //            CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //            PostType = table.Column<int>(type: "int", nullable: false),
            //            EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
            //            StartAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
            //            CurrentAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
            //            CurrentBidderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            //            AuctionStatus = table.Column<int>(type: "int", nullable: true),
            //            CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            //            UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_Posts", x => x.Id);
            //            table.ForeignKey(
            //                name: "FK_Posts_Categories_CategoryId",
            //                column: x => x.CategoryId,
            //                principalTable: "Categories",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Cascade);
            //            table.ForeignKey(
            //                name: "FK_Posts_Users_UserId",
            //                column: x => x.UserId,
            //                principalTable: "Users",
            //                principalColumn: "Id");
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "Bids",
            //        columns: table => new
            //        {
            //            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //            PostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //            BidderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //            Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            //            BidTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            //            CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            //            UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_Bids", x => x.Id);
            //            table.ForeignKey(
            //                name: "FK_Bids_Posts_PostId",
            //                column: x => x.PostId,
            //                principalTable: "Posts",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Cascade);
            //            table.ForeignKey(
            //                name: "FK_Bids_Users_BidderId",
            //                column: x => x.BidderId,
            //                principalTable: "Users",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Cascade);
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "WatchLists",
            //        columns: table => new
            //        {
            //            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //            UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //            PostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //            CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            //            UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_WatchLists", x => x.Id);
            //            table.ForeignKey(
            //                name: "FK_WatchLists_Posts_PostId",
            //                column: x => x.PostId,
            //                principalTable: "Posts",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Cascade);
            //            table.ForeignKey(
            //                name: "FK_WatchLists_Users_UserId",
            //                column: x => x.UserId,
            //                principalTable: "Users",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Cascade);
            //        });

            //    migrationBuilder.CreateIndex(
            //        name: "IX_Bids_BidderId",
            //        table: "Bids",
            //        column: "BidderId");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_Bids_PostId",
            //        table: "Bids",
            //        column: "PostId");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_Notifications_UserId",
            //        table: "Notifications",
            //        column: "UserId");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_Posts_CategoryId",
            //        table: "Posts",
            //        column: "CategoryId");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_Posts_UserId",
            //        table: "Posts",
            //        column: "UserId");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_WatchLists_PostId",
            //        table: "WatchLists",
            //        column: "PostId");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_WatchLists_UserId",
            //        table: "WatchLists",
            //        column: "UserId");

            //    migrationBuilder.AddForeignKey(
            //        name: "FK_ChatUsers_Users_UserId",
            //        table: "ChatUsers",
            //        column: "UserId",
            //        principalTable: "Users",
            //        principalColumn: "Id",
            //        onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatUsers_Users_UserId",
                table: "ChatUsers");

            migrationBuilder.DropTable(
                name: "Bids");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "WatchLists");

            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AccountName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Avatar",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PasswordReset",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PasswordSalt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ResetToken",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ResetTokenExpires",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Users");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "User");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "User",
                newName: "TempId1");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_User_TempId1",
                table: "User",
                column: "TempId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatUsers_User_UserId",
                table: "ChatUsers",
                column: "UserId",
                principalTable: "User",
                principalColumn: "TempId1",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
