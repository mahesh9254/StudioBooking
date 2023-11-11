using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudioBooking.Migrations
{
    public partial class _2023111116020_AddRequestEndDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "RequestEndDate",
                table: "ScheduleRequests",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequestEndDate",
                table: "ScheduleRequests");
        }
    }
}
