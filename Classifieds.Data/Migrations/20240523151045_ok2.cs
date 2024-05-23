using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Classifieds.Data.Migrations
{
    /// <inheritdoc />
    public partial class ok2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Users_CurrentBidderId",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_CurrentBidderId",
                table: "Posts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Posts_CurrentBidderId",
                table: "Posts",
                column: "CurrentBidderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Users_CurrentBidderId",
                table: "Posts",
                column: "CurrentBidderId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
