using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend3.Migrations
{
    public partial class ReviewIdAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Review_Event_EventId",
                table: "Review");

            migrationBuilder.DropIndex(
                name: "IX_Review_EventId",
                table: "Review");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Review",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Review",
                table: "Review",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Review",
                table: "Review");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Review");

            migrationBuilder.CreateIndex(
                name: "IX_Review_EventId",
                table: "Review",
                column: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_Review_Event_EventId",
                table: "Review",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
