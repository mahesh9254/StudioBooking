using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudioBooking.Migrations
{
    public partial class AddedAccountDetailFieldsInScheduleRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Account",
                table: "ScheduleRequests",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IFSC",
                table: "ScheduleRequests",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ScheduleRequests",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentProvider",
                table: "ScheduleRequests",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Account",
                table: "ScheduleRequests");

            migrationBuilder.DropColumn(
                name: "IFSC",
                table: "ScheduleRequests");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "ScheduleRequests");

            migrationBuilder.DropColumn(
                name: "PaymentProvider",
                table: "ScheduleRequests");
        }
    }
}
