using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthcareInventoryManagementSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemovePurchaseOrderUserTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrders_Users_ApprovedBy",
                table: "PurchaseOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrders_Users_CreatedBy",
                table: "PurchaseOrders");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseOrders_ApprovedBy",
                table: "PurchaseOrders");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseOrders_CreatedBy",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "PurchaseOrders");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ApprovedBy",
                table: "PurchaseOrders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "PurchaseOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_ApprovedBy",
                table: "PurchaseOrders",
                column: "ApprovedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_CreatedBy",
                table: "PurchaseOrders",
                column: "CreatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrders_Users_ApprovedBy",
                table: "PurchaseOrders",
                column: "ApprovedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrders_Users_CreatedBy",
                table: "PurchaseOrders",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
