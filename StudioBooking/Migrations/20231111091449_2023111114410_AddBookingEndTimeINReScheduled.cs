using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudioBooking.Migrations
{
    public partial class _2023111114410_AddBookingEndTimeINReScheduled : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "BookingEndDate",
                table: "ScheduleRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BookingEndDate",
                table: "Carts",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BookingEndDate",
                table: "ScheduleRequests");

            migrationBuilder.DropColumn(
                name: "BookingEndDate",
                table: "Carts");
        }
    }
}
