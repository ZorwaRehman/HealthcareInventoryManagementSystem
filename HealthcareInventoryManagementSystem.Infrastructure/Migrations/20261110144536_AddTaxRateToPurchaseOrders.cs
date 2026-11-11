using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthcareInventoryManagementSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTaxRateToPurchaseOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TaxRate",
                table: "PurchaseOrders",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TaxRate",
                table: "PurchaseOrders");
        }
    }
}
