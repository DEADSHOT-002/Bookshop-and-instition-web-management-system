using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookshopTuitionSystem.Migrations
{
    /// <inheritdoc />
    public partial class UpgradeInventoryModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Stock",
                table: "Items",
                newName: "Quantity");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "Items",
                newName: "SellingPrice");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Items",
                newName: "Type");

            migrationBuilder.AddColumn<string>(
                name: "Brand",
                table: "Items",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "CostPrice",
                table: "Items",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "DatePurchased",
                table: "Items",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "PurchaseReceiptNo",
                table: "Items",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Supplier",
                table: "Items",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Brand",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "CostPrice",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "DatePurchased",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "PurchaseReceiptNo",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Supplier",
                table: "Items");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Items",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "SellingPrice",
                table: "Items",
                newName: "Price");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "Items",
                newName: "Stock");
        }
    }
}
