using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudioBooking.Migrations
{
    public partial class AddedCCAvenueFieldsInPaymentGatewayResponse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PaymentSsessionId",
                table: "PaymentGatewayResponses",
                newName: "StatusMessage");

            migrationBuilder.RenameColumn(
                name: "CashFreeOrderId",
                table: "PaymentGatewayResponses",
                newName: "PaymentSessionId");

            migrationBuilder.AddColumn<string>(
                name: "BankRefNo",
                table: "PaymentGatewayResponses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CardName",
                table: "PaymentGatewayResponses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FailureMessage",
                table: "PaymentGatewayResponses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrderId",
                table: "PaymentGatewayResponses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentMode",
                table: "PaymentGatewayResponses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StatusCode",
                table: "PaymentGatewayResponses",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BankRefNo",
                table: "PaymentGatewayResponses");

            migrationBuilder.DropColumn(
                name: "CardName",
                table: "PaymentGatewayResponses");

            migrationBuilder.DropColumn(
                name: "FailureMessage",
                table: "PaymentGatewayResponses");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "PaymentGatewayResponses");

            migrationBuilder.DropColumn(
                name: "PaymentMode",
                table: "PaymentGatewayResponses");

            migrationBuilder.DropColumn(
                name: "StatusCode",
                table: "PaymentGatewayResponses");

            migrationBuilder.RenameColumn(
                name: "StatusMessage",
                table: "PaymentGatewayResponses",
                newName: "PaymentSsessionId");

            migrationBuilder.RenameColumn(
                name: "PaymentSessionId",
                table: "PaymentGatewayResponses",
                newName: "CashFreeOrderId");
        }
    }
}
