
using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthcareInventoryManagementSystem.Infrastructure.Migrations
{
    public partial class LinkMedicineToItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Remove batch information because it will belong
            // to the future StockBatch table.
            migrationBuilder.DropColumn(
                name: "BatchNumber",
                table: "Medicines");

            migrationBuilder.DropColumn(
                name: "ExpiryDate",
                table: "Medicines");

            // The medicine name is stored in the Items table.
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Medicines");

            migrationBuilder.DropColumn(
                name: "QuantityInStock",
                table: "Medicines");

            migrationBuilder.DropColumn(
                name: "UnitPrice",
                table: "Medicines");

            // Remove ReorderLevel because it belongs to Items.
            migrationBuilder.DropColumn(
                name: "ReorderLevel",
                table: "Medicines");

            // Add a new column to link MedicineEntity to ItemEntity.
            migrationBuilder.AddColumn<int>(
                name: "ItemId",
                table: "Medicines",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // Enforce one medicine profile per item.
            migrationBuilder.CreateIndex(
                name: "IX_Medicines_ItemId",
                table: "Medicines",
                column: "ItemId",
                unique: true);

            // Link Medicines.ItemId to Items.Id.
            migrationBuilder.AddForeignKey(
                name: "FK_Medicines_Items_ItemId",
                table: "Medicines",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove the foreign key first.
            migrationBuilder.DropForeignKey(
                name: "FK_Medicines_Items_ItemId",
                table: "Medicines");

            migrationBuilder.DropIndex(
                name: "IX_Medicines_ItemId",
                table: "Medicines");

            // Remove the newly added ItemId column.
            migrationBuilder.DropColumn(
                name: "ItemId",
                table: "Medicines");

            // Restore ReorderLevel.
            migrationBuilder.AddColumn<int>(
                name: "ReorderLevel",
                table: "Medicines",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // Restore the previous columns.
            migrationBuilder.AddColumn<string>(
                name: "BatchNumber",
                table: "Medicines",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiryDate",
                table: "Medicines",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(
                    1, 1, 1, 0, 0, 0, 0,
                    DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Medicines",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "QuantityInStock",
                table: "Medicines",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "UnitPrice",
                table: "Medicines",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }
    }
}
