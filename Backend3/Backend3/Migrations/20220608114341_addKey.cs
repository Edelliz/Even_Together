using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend3.Migrations
{
    public partial class addKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UsersEvents_EventId",
                table: "UsersEvents");

            migrationBuilder.DropIndex(
                name: "IX_Searching_EventId",
                table: "Searching");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsersEvents",
                table: "UsersEvents",
                columns: new[] { "EventId", "UserId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Searching",
                table: "Searching",
                columns: new[] { "EventId", "UserId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UsersEvents",
                table: "UsersEvents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Searching",
                table: "Searching");

            migrationBuilder.CreateIndex(
                name: "IX_UsersEvents_EventId",
                table: "UsersEvents",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Searching_EventId",
                table: "Searching",
                column: "EventId");
        }
    }
}
