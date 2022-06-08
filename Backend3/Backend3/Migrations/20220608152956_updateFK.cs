using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend3.Migrations
{
    public partial class updateFK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Searching_Users_EventId",
                table: "Searching");

            migrationBuilder.CreateIndex(
                name: "IX_Searching_UserId",
                table: "Searching",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Searching_Users_UserId",
                table: "Searching",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Searching_Users_UserId",
                table: "Searching");

            migrationBuilder.DropIndex(
                name: "IX_Searching_UserId",
                table: "Searching");

            migrationBuilder.AddForeignKey(
                name: "FK_Searching_Users_EventId",
                table: "Searching",
                column: "EventId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
