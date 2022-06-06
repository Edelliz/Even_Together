using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend3.Migrations
{
    public partial class editEvent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Searching_Event_EventId",
                table: "Searching");

            migrationBuilder.DropForeignKey(
                name: "FK_Searching_Users_EventId",
                table: "Searching");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersEvents_Event_EventId",
                table: "UsersEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersEvents_Users_UserId",
                table: "UsersEvents");

            migrationBuilder.AddColumn<int>(
                name: "Grade",
                table: "Event",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Place",
                table: "Event",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Review",
                columns: table => new
                {
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK_Review_Event_EventId",
                        column: x => x.EventId,
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Review_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Review_EventId",
                table: "Review",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Review_OwnerId",
                table: "Review",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Searching_Event_EventId",
                table: "Searching",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Searching_Users_EventId",
                table: "Searching",
                column: "EventId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsersEvents_Event_EventId",
                table: "UsersEvents",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsersEvents_Users_UserId",
                table: "UsersEvents",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Searching_Event_EventId",
                table: "Searching");

            migrationBuilder.DropForeignKey(
                name: "FK_Searching_Users_EventId",
                table: "Searching");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersEvents_Event_EventId",
                table: "UsersEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersEvents_Users_UserId",
                table: "UsersEvents");

            migrationBuilder.DropTable(
                name: "Review");

            migrationBuilder.DropColumn(
                name: "Grade",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "Place",
                table: "Event");

            migrationBuilder.AddForeignKey(
                name: "FK_Searching_Event_EventId",
                table: "Searching",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Searching_Users_EventId",
                table: "Searching",
                column: "EventId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UsersEvents_Event_EventId",
                table: "UsersEvents",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UsersEvents_Users_UserId",
                table: "UsersEvents",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
