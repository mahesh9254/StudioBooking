using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudioBooking.Migrations
{
    public partial class _2023111108311_AddBookingEndTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "BookingEndDate",
                table: "Bookings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BookingEndDate",
                table: "Bookings");
        }
    }
}
