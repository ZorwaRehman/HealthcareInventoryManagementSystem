using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthcareInventoryManagementSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGoodsReceived : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GoodsReceived",
                columns: table => new
                {
                    GoodsReceivedId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReceiptNumber = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    PurchaseOrderId = table.Column<int>(type: "int", nullable: false),
                    ReceivedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeliveryNoteNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ConfirmedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoodsReceived", x => x.GoodsReceivedId);
                    table.ForeignKey(
                        name: "FK_GoodsReceived_PurchaseOrders_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalTable: "PurchaseOrders",
                        principalColumn: "PurchaseOrderId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GoodsReceivedItems",
                columns: table => new
                {
                    GoodsReceivedItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GoodsReceivedId = table.Column<int>(type: "int", nullable: false),
                    PurchaseOrderItemId = table.Column<int>(type: "int", nullable: false),
                    BatchNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DeliveredQuantity = table.Column<int>(type: "int", nullable: false),
                    AcceptedQuantity = table.Column<int>(type: "int", nullable: false),
                    RejectedQuantity = table.Column<int>(type: "int", nullable: false),
                    QuarantinedQuantity = table.Column<int>(type: "int", nullable: false),
                    ManufactureDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StorageLocation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    StockBatchId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoodsReceivedItems", x => x.GoodsReceivedItemId);
                    table.ForeignKey(
                        name: "FK_GoodsReceivedItems_GoodsReceived_GoodsReceivedId",
                        column: x => x.GoodsReceivedId,
                        principalTable: "GoodsReceived",
                        principalColumn: "GoodsReceivedId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GoodsReceivedItems_PurchaseOrderItems_PurchaseOrderItemId",
                        column: x => x.PurchaseOrderItemId,
                        principalTable: "PurchaseOrderItems",
                        principalColumn: "PurchaseOrderItemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GoodsReceivedItems_StockBatches_StockBatchId",
                        column: x => x.StockBatchId,
                        principalTable: "StockBatches",
                        principalColumn: "StockBatchId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceived_PurchaseOrderId",
                table: "GoodsReceived",
                column: "PurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceived_ReceiptNumber",
                table: "GoodsReceived",
                column: "ReceiptNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceived_ReceivedDate",
                table: "GoodsReceived",
                column: "ReceivedDate");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceived_Status",
                table: "GoodsReceived",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceivedItems_GoodsReceivedId",
                table: "GoodsReceivedItems",
                column: "GoodsReceivedId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceivedItems_PurchaseOrderItemId",
                table: "GoodsReceivedItems",
                column: "PurchaseOrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceivedItems_StockBatchId",
                table: "GoodsReceivedItems",
                column: "StockBatchId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GoodsReceivedItems");

            migrationBuilder.DropTable(
                name: "GoodsReceived");
        }
    }
}
