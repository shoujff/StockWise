using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace StockWise.App.Migrations
{
    /// <inheritdoc />
    public partial class SeedExtendedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Customer",
                columns: new[] { "Id", "ContactPerson", "Email", "INN", "Name", "Phone" },
                values: new object[,]
                {
                    { 2, "Петр Петров", "info@technomir.ru", "7702345678", "ООО ТехноМир", "+7 (495) 234-56-78" },
                    { 3, "Алексей Сидоров", "sidorov@mail.ru", "7703456789", "ИП Сидоров А.В.", "+7 (495) 345-67-89" }
                });

            migrationBuilder.InsertData(
                table: "Inventory",
                columns: new[] { "Id", "CreatedBy", "Date", "Number", "Status", "TotalDiff", "UserId", "WarehouseId" },
                values: new object[] { 1, 3, new DateTime(2026, 7, 10, 9, 0, 0, 0, DateTimeKind.Utc), "INV-2026-0001", "Draft", -3m, 3, 1 });

            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "CustomerId", "Number", "Status", "TotalAmount" },
                values: new object[] { 1, new DateTime(2026, 7, 5, 10, 0, 0, 0, DateTimeKind.Utc), 2, 1, "ORD-2026-0001", "New", 17000m });

            migrationBuilder.InsertData(
                table: "InventoryLine",
                columns: new[] { "Id", "ActualQty", "BatchNo", "Diff", "ExpectedQty", "InventoryId", "ItemId", "Price" },
                values: new object[,]
                {
                    { 1, 28m, null, -2m, 30m, 1, 1, 350m },
                    { 2, 6m, null, 0m, 6m, 1, 5, 3500m },
                    { 3, 4m, null, -1m, 5m, 1, 8, 500m }
                });

            migrationBuilder.InsertData(
                table: "OrderLines",
                columns: new[] { "Id", "Amount", "ItemId", "OrderId", "Price", "Quantity", "ShippedQty" },
                values: new object[,]
                {
                    { 1, 5000m, 2, 1, 2500m, 2m, 0m },
                    { 2, 12000m, 4, 1, 12000m, 1m, 0m }
                });

            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "CustomerId", "Number", "Status", "TotalAmount" },
                values: new object[,]
                {
                    { 2, new DateTime(2026, 7, 7, 14, 0, 0, 0, DateTimeKind.Utc), 1, 2, "ORD-2026-0002", "InProgress", 8400m },
                    { 3, new DateTime(2026, 7, 9, 9, 0, 0, 0, DateTimeKind.Utc), 2, 3, "ORD-2026-0003", "Cancelled", 1750m }
                });

            migrationBuilder.InsertData(
                table: "Reservations",
                columns: new[] { "Id", "CreatedAt", "OrderId", "Quantity", "Status", "StockBalanceId" },
                values: new object[,]
                {
                    { 1L, new DateTime(2026, 7, 5, 10, 0, 0, 0, DateTimeKind.Utc), 1, 2m, "Active", 2L },
                    { 2L, new DateTime(2026, 7, 5, 10, 0, 0, 0, DateTimeKind.Utc), 1, 1m, "Active", 4L }
                });

            migrationBuilder.InsertData(
                table: "OrderLines",
                columns: new[] { "Id", "Amount", "ItemId", "OrderId", "Price", "Quantity", "ShippedQty" },
                values: new object[,]
                {
                    { 3, 8400m, 6, 2, 2800m, 3m, 0m },
                    { 4, 1750m, 1, 3, 350m, 5m, 0m }
                });

            migrationBuilder.InsertData(
                table: "Reservations",
                columns: new[] { "Id", "CreatedAt", "OrderId", "Quantity", "Status", "StockBalanceId" },
                values: new object[,]
                {
                    { 3L, new DateTime(2026, 7, 7, 14, 0, 0, 0, DateTimeKind.Utc), 2, 3m, "Active", 6L },
                    { 4L, new DateTime(2026, 7, 9, 9, 0, 0, 0, DateTimeKind.Utc), 3, 5m, "Released", 1L }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "InventoryLine",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "InventoryLine",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "InventoryLine",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "OrderLines",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "OrderLines",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "OrderLines",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "OrderLines",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Reservations",
                keyColumn: "Id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "Reservations",
                keyColumn: "Id",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "Reservations",
                keyColumn: "Id",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "Reservations",
                keyColumn: "Id",
                keyValue: 4L);

            migrationBuilder.DeleteData(
                table: "Inventory",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Customer",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Customer",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
