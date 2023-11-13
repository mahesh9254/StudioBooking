using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudioBooking.Migrations
{
    public partial class _2023111317420_ModifyCoupon : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Coupons_Categories_CategoryId",
                table: "Coupons");

            migrationBuilder.DropForeignKey(
                name: "FK_Coupons_Services_ServiceId",
                table: "Coupons");

            migrationBuilder.DropIndex(
                name: "IX_Coupons_CategoryId",
                table: "Coupons");

            migrationBuilder.DropIndex(
                name: "IX_Coupons_ServiceId",
                table: "Coupons");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Coupons");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "Coupons");

            migrationBuilder.DropColumn(
                name: "ServiceId",
                table: "Coupons");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "Coupons");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Coupons",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Coupons");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Coupons",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "EndTime",
                table: "Coupons",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ServiceId",
                table: "Coupons",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "StartTime",
                table: "Coupons",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_CategoryId",
                table: "Coupons",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_ServiceId",
                table: "Coupons",
                column: "ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Coupons_Categories_CategoryId",
                table: "Coupons",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Coupons_Services_ServiceId",
                table: "Coupons",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
