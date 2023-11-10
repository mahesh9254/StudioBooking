using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudioBooking.Migrations
{
    public partial class AddedPaymentGatewayResponseEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PaymentGatewayResponses",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionId = table.Column<long>(type: "bigint", nullable: true),
                    BookingId = table.Column<long>(type: "bigint", nullable: true),
                    CashFreeOrderId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentSsessionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderAmount = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderCurrency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderExpiryTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OrderNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderStatus = table.Column<int>(type: "int", nullable: true),
                    PaymentUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefundUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SettlementUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentGatewayResponses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentGatewayResponses_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PaymentGatewayResponses_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentGatewayResponses_BookingId",
                table: "PaymentGatewayResponses",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentGatewayResponses_TransactionId",
                table: "PaymentGatewayResponses",
                column: "TransactionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentGatewayResponses");
        }
    }
}
