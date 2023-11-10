using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudioBooking.Migrations
{
    public partial class AddedEmailSettingsInWebsiteSettingEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmailAccount",
                table: "WebsiteSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmailAccountPassword",
                table: "WebsiteSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmailAccountSmtp",
                table: "WebsiteSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmailAccountUserId",
                table: "WebsiteSettings",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailAccount",
                table: "WebsiteSettings");

            migrationBuilder.DropColumn(
                name: "EmailAccountPassword",
                table: "WebsiteSettings");

            migrationBuilder.DropColumn(
                name: "EmailAccountSmtp",
                table: "WebsiteSettings");

            migrationBuilder.DropColumn(
                name: "EmailAccountUserId",
                table: "WebsiteSettings");
        }
    }
}
