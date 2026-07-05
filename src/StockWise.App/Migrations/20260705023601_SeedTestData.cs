using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace StockWise.App.Migrations
{
    /// <inheritdoc />
    public partial class SeedTestData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Customer",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Customer",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "INN",
                table: "Customer",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Customer",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ContactPerson",
                table: "Customer",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name", "ParentId", "SortOrder" },
                values: new object[,]
                {
                    { 1, "Электроника", null, 1 },
                    { 5, "Строительные материалы", null, 2 },
                    { 9, "Канцелярия", null, 3 }
                });

            migrationBuilder.InsertData(
                table: "Customer",
                columns: new[] { "Id", "ContactPerson", "Email", "INN", "Name", "Phone" },
                values: new object[] { 1, "Иван Иванов", "info@roznica.ru", "7701234567", "ООО Розница", "+7 (495) 123-45-67" });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "Id", "Permission", "Role" },
                values: new object[,]
                {
                    { 1, "Items.Create", "Admin" },
                    { 2, "Items.Edit", "Admin" },
                    { 3, "Items.Delete", "Admin" },
                    { 4, "Items.View", "Admin" },
                    { 5, "Warehouse.Create", "Admin" },
                    { 6, "Warehouse.Edit", "Admin" },
                    { 7, "Warehouse.View", "Admin" },
                    { 8, "Documents.Create", "Admin" },
                    { 9, "Documents.Post", "Admin" },
                    { 10, "Documents.View", "Admin" },
                    { 11, "Documents.Cancel", "Admin" },
                    { 12, "Orders.Create", "Admin" },
                    { 13, "Orders.Edit", "Admin" },
                    { 14, "Orders.View", "Admin" },
                    { 15, "Reports.View", "Admin" },
                    { 16, "Admin.Users", "Admin" },
                    { 17, "Items.Create", "Manager" },
                    { 18, "Items.Edit", "Manager" },
                    { 19, "Items.View", "Manager" },
                    { 20, "Documents.Create", "Manager" },
                    { 21, "Documents.Post", "Manager" },
                    { 22, "Documents.View", "Manager" },
                    { 23, "Orders.Create", "Manager" },
                    { 24, "Orders.Edit", "Manager" },
                    { 25, "Orders.View", "Manager" },
                    { 26, "Reports.View", "Manager" },
                    { 27, "Items.View", "Warehouse" },
                    { 28, "Warehouse.View", "Warehouse" },
                    { 29, "Documents.View", "Warehouse" },
                    { 30, "Inventory.Create", "Warehouse" },
                    { 31, "Inventory.Edit", "Warehouse" },
                    { 32, "Items.View", "Viewer" },
                    { 33, "Warehouse.View", "Viewer" },
                    { 34, "Documents.View", "Viewer" },
                    { 35, "Reports.View", "Viewer" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "FirstName", "LastName", "Login", "PasswordHash", "Role" },
                values: new object[,]
                {
                    { 1, "Администратор", "", "admin", "$2a$10$v7tufXv/BzNs2LFs1YlNX.M6limATLB.C5ZySCXDiK4ev8D/Nu4Qy", "Admin" },
                    { 2, "Иван", "Петров", "manager", "$2a$10$b/irvmKoiR3SRXJKOFpZUebCb9267sh9lMWDroW1rgtk5T2ByUXlK", "Manager" },
                    { 3, "Сергей", "Иванов", "warehouse", "$2a$10$aTcj/v7LGvvOc4r1Qs5nqOm7wUMrFKjO4luqNBSfRlU0Ul3uJBcXK", "Warehouse" },
                    { 4, "Анна", "Сидорова", "viewer", "$2a$10$.syaB/VMqXxB.xM8ZF4FBOef/sqnbAuXXLzkXCJZ8pwLm3QJgVI1y", "Viewer" }
                });

            migrationBuilder.InsertData(
                table: "Warehouses",
                columns: new[] { "Id", "Address", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, "ул. Ленина, 10", true, "Основной склад" },
                    { 2, "ул. Промышленная, 5", true, "Склад №2" },
                    { 3, "ТЦ \"Гигант\", пав. 12", true, "Мелкооптовый" }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name", "ParentId", "SortOrder" },
                values: new object[,]
                {
                    { 2, "Бытовая техника", 1, 1 },
                    { 3, "Компьютеры", 1, 2 },
                    { 4, "Телефоны", 1, 3 },
                    { 6, "Электрика", 5, 1 },
                    { 7, "Сантехника", 5, 2 },
                    { 8, "Отделочные материалы", 5, 3 },
                    { 10, "Бумага", 9, 1 },
                    { 11, "Ручки", 9, 2 }
                });

            migrationBuilder.InsertData(
                table: "Documents",
                columns: new[] { "Id", "CreatedBy", "CustomerId", "Date", "FromWarehouseId", "Number", "Status", "StockRefDocId", "SupplierName", "ToWarehouseId", "TotalAmount", "Type" },
                values: new object[,]
                {
                    { 1, 1, null, new DateTime(2026, 7, 1, 10, 0, 0, 0, DateTimeKind.Utc), null, "IN-2026-0001", "Posted", null, "ООО Электротех", 1, 37000m, "Income" },
                    { 2, 2, 1, new DateTime(2026, 7, 2, 14, 0, 0, 0, DateTimeKind.Utc), 1, "OUT-2026-0001", "Posted", null, null, null, 9500m, "Outcome" },
                    { 3, 1, null, new DateTime(2026, 7, 3, 9, 0, 0, 0, DateTimeKind.Utc), 1, "TRF-2026-0001", "Draft", null, null, 2, 0m, "Transfer" }
                });

            migrationBuilder.InsertData(
                table: "Items",
                columns: new[] { "Id", "Article", "Barcode", "CategoryId", "CreatedAt", "ImagePath", "IsBatch", "MaxStock", "MinStock", "Name", "Price", "Unit" },
                values: new object[,]
                {
                    { 1, "HDMI-001", null, 3, new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 50m, 5m, "HDMI кабель 3м", 350m, "шт" },
                    { 2, "USB-HUB-001", null, 3, new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 20m, 2m, "USB-C Hub 7in1", 2500m, "шт" },
                    { 3, "BOSCH-T-001", null, 2, new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 10m, 2m, "Чайник Bosch TWK7201", 4500m, "шт" },
                    { 4, "SAMS-MW-001", null, 2, new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 5m, 1m, "Микроволновка Samsung MW3500", 12000m, "шт" },
                    { 5, "LOG-KB-001", null, 3, new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 15m, 3m, "Клавиатура Logitech K480", 3500m, "шт" },
                    { 6, "RAZER-MS-001", null, 3, new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 20m, 3m, "Мышь Razer DeathAdder", 2800m, "шт" },
                    { 7, "DELL-MON-001", null, 3, new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 5m, 1m, "Монитор Dell 27\" S2722QC", 35000m, "шт" },
                    { 8, "IP15-CASE-001", null, 4, new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 50m, 10m, "Чехол iPhone 15 Pro", 500m, "шт" },
                    { 9, "IP15-GLASS-001", null, 4, new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 100m, 20m, "Защитное стекло iPhone 15 Pro", 300m, "шт" },
                    { 10, "EXT-5M-001", null, 6, new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 30m, 5m, "Удлинитель 5м 6 розеток", 850m, "шт" },
                    { 11, "LED-12W-001", null, 6, new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, 100m, 20m, "Лампочка LED 12W", 150m, "шт" },
                    { 12, "GROHE-MIX-001", null, 7, new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 10m, 2m, "Смеситель Grohe Eurosmart", 3500m, "шт" },
                    { 13, "HOSE-1.5-001", null, 7, new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 30m, 5m, "Шланг душевой 1.5м", 450m, "шт" },
                    { 14, "VETONIT-001", null, 8, new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 30m, 5m, "Шпатлёвка Vetonit 5кг", 350m, "меш" },
                    { 15, "PAINT-WH-001", null, 8, new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 15m, 3m, "Краска белая 3л", 1200m, "бан" },
                    { 16, "PAPER-A4-001", null, 10, new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 50m, 10m, "Бумага А4 \"Снегурочка\" 500л", 550m, "пач" },
                    { 17, "PEN-BLUE-001", null, 11, new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 200m, 50m, "Ручка шариковая синяя", 25m, "шт" },
                    { 18, "MARKER-BLK-001", null, 11, new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 100m, 20m, "Маркер перманентный чёрный", 85m, "шт" }
                });

            migrationBuilder.InsertData(
                table: "DocumentLines",
                columns: new[] { "Id", "Amount", "BatchNo", "DocumentId", "ExpiryDate", "ItemId", "Price", "Quantity" },
                values: new object[,]
                {
                    { 1, 7000m, null, 1, null, 1, 350m, 20m },
                    { 2, 25000m, null, 1, null, 2, 2500m, 10m },
                    { 3, 5000m, null, 1, null, 8, 500m, 10m },
                    { 4, 7000m, null, 2, null, 5, 3500m, 2m },
                    { 5, 2500m, null, 2, null, 8, 500m, 5m },
                    { 6, 3500m, null, 3, null, 1, 350m, 10m },
                    { 7, 7500m, null, 3, null, 2, 2500m, 3m }
                });

            migrationBuilder.InsertData(
                table: "StockBalances",
                columns: new[] { "Id", "BatchNo", "ExpiryDate", "ItemId", "Price", "Quantity", "UpdatedAt", "WarehouseId" },
                values: new object[] { 1L, null, null, 1, 350m, 30m, new DateTime(2026, 7, 1, 10, 0, 0, 0, DateTimeKind.Utc), 1 });

            migrationBuilder.InsertData(
                table: "StockBalances",
                columns: new[] { "Id", "BatchNo", "ExpiryDate", "ItemId", "Price", "Quantity", "ReservedQty", "UpdatedAt", "WarehouseId" },
                values: new object[] { 2L, null, null, 2, 2500m, 10m, 2m, new DateTime(2026, 7, 1, 10, 0, 0, 0, DateTimeKind.Utc), 1 });

            migrationBuilder.InsertData(
                table: "StockBalances",
                columns: new[] { "Id", "BatchNo", "ExpiryDate", "ItemId", "Price", "Quantity", "UpdatedAt", "WarehouseId" },
                values: new object[] { 3L, null, null, 3, 4500m, 5m, new DateTime(2026, 7, 1, 10, 0, 0, 0, DateTimeKind.Utc), 1 });

            migrationBuilder.InsertData(
                table: "StockBalances",
                columns: new[] { "Id", "BatchNo", "ExpiryDate", "ItemId", "Price", "Quantity", "ReservedQty", "UpdatedAt", "WarehouseId" },
                values: new object[] { 4L, null, null, 4, 12000m, 2m, 1m, new DateTime(2026, 7, 1, 10, 0, 0, 0, DateTimeKind.Utc), 1 });

            migrationBuilder.InsertData(
                table: "StockBalances",
                columns: new[] { "Id", "BatchNo", "ExpiryDate", "ItemId", "Price", "Quantity", "UpdatedAt", "WarehouseId" },
                values: new object[] { 5L, null, null, 5, 3500m, 6m, new DateTime(2026, 7, 2, 14, 0, 0, 0, DateTimeKind.Utc), 1 });

            migrationBuilder.InsertData(
                table: "StockBalances",
                columns: new[] { "Id", "BatchNo", "ExpiryDate", "ItemId", "Price", "Quantity", "ReservedQty", "UpdatedAt", "WarehouseId" },
                values: new object[] { 6L, null, null, 6, 2800m, 15m, 3m, new DateTime(2026, 7, 1, 10, 0, 0, 0, DateTimeKind.Utc), 2 });

            migrationBuilder.InsertData(
                table: "StockBalances",
                columns: new[] { "Id", "BatchNo", "ExpiryDate", "ItemId", "Price", "Quantity", "UpdatedAt", "WarehouseId" },
                values: new object[,]
                {
                    { 7L, null, null, 7, 35000m, 3m, new DateTime(2026, 7, 1, 10, 0, 0, 0, DateTimeKind.Utc), 2 },
                    { 8L, null, null, 8, 500m, 5m, new DateTime(2026, 7, 2, 14, 0, 0, 0, DateTimeKind.Utc), 1 },
                    { 9L, null, null, 9, 300m, 60m, new DateTime(2026, 7, 1, 10, 0, 0, 0, DateTimeKind.Utc), 1 },
                    { 10L, null, null, 10, 850m, 15m, new DateTime(2026, 7, 1, 10, 0, 0, 0, DateTimeKind.Utc), 1 },
                    { 11L, "LED-2026-01", new DateOnly(2026, 12, 31), 11, 150m, 50m, new DateTime(2026, 7, 1, 10, 0, 0, 0, DateTimeKind.Utc), 1 },
                    { 12L, "LED-2026-02", new DateOnly(2027, 6, 30), 11, 160m, 30m, new DateTime(2026, 7, 1, 10, 0, 0, 0, DateTimeKind.Utc), 1 },
                    { 13L, null, null, 12, 3500m, 4m, new DateTime(2026, 7, 1, 10, 0, 0, 0, DateTimeKind.Utc), 2 },
                    { 14L, null, null, 13, 450m, 12m, new DateTime(2026, 7, 1, 10, 0, 0, 0, DateTimeKind.Utc), 2 },
                    { 15L, null, null, 14, 350m, 8m, new DateTime(2026, 7, 1, 10, 0, 0, 0, DateTimeKind.Utc), 2 },
                    { 16L, null, null, 15, 1200m, 3m, new DateTime(2026, 7, 1, 10, 0, 0, 0, DateTimeKind.Utc), 2 },
                    { 17L, null, null, 16, 550m, 20m, new DateTime(2026, 7, 1, 10, 0, 0, 0, DateTimeKind.Utc), 1 },
                    { 18L, null, null, 16, 520m, 30m, new DateTime(2026, 7, 1, 10, 0, 0, 0, DateTimeKind.Utc), 3 },
                    { 19L, null, null, 17, 25m, 150m, new DateTime(2026, 7, 1, 10, 0, 0, 0, DateTimeKind.Utc), 3 },
                    { 20L, null, null, 18, 85m, 80m, new DateTime(2026, 7, 1, 10, 0, 0, 0, DateTimeKind.Utc), 3 },
                    { 21L, null, null, 1, 350m, 20m, new DateTime(2026, 7, 1, 10, 0, 0, 0, DateTimeKind.Utc), 2 }
                });

            migrationBuilder.InsertData(
                table: "Transactions",
                columns: new[] { "Id", "BatchNo", "CreatedAt", "CreatedBy", "Direction", "ExpiryDate", "ItemId", "Price", "Quantity", "RefDocId", "RefDocType", "StockBalanceId", "Type", "WarehouseId" },
                values: new object[,]
                {
                    { 1001L, null, new DateTime(2026, 7, 1, 10, 0, 0, 0, DateTimeKind.Utc), 1, "+", null, 1, 350m, 20m, 1, "Income", null, "Income", 1 },
                    { 1002L, null, new DateTime(2026, 7, 1, 10, 0, 0, 0, DateTimeKind.Utc), 1, "+", null, 2, 2500m, 10m, 1, "Income", null, "Income", 1 },
                    { 1003L, null, new DateTime(2026, 7, 1, 10, 0, 0, 0, DateTimeKind.Utc), 1, "+", null, 8, 500m, 10m, 1, "Income", null, "Income", 1 },
                    { 1004L, null, new DateTime(2026, 7, 2, 14, 0, 0, 0, DateTimeKind.Utc), 2, "-", null, 5, 3500m, 2m, 2, "Outcome", null, "Outcome", 1 },
                    { 1005L, null, new DateTime(2026, 7, 2, 14, 0, 0, 0, DateTimeKind.Utc), 2, "-", null, 8, 500m, 5m, 2, "Outcome", null, "Outcome", 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "DocumentLines",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "DocumentLines",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "DocumentLines",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "DocumentLines",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "DocumentLines",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "DocumentLines",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "DocumentLines",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "StockBalances",
                keyColumn: "Id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "StockBalances",
                keyColumn: "Id",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "StockBalances",
                keyColumn: "Id",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "StockBalances",
                keyColumn: "Id",
                keyValue: 4L);

            migrationBuilder.DeleteData(
                table: "StockBalances",
                keyColumn: "Id",
                keyValue: 5L);

            migrationBuilder.DeleteData(
                table: "StockBalances",
                keyColumn: "Id",
                keyValue: 6L);

            migrationBuilder.DeleteData(
                table: "StockBalances",
                keyColumn: "Id",
                keyValue: 7L);

            migrationBuilder.DeleteData(
                table: "StockBalances",
                keyColumn: "Id",
                keyValue: 8L);

            migrationBuilder.DeleteData(
                table: "StockBalances",
                keyColumn: "Id",
                keyValue: 9L);

            migrationBuilder.DeleteData(
                table: "StockBalances",
                keyColumn: "Id",
                keyValue: 10L);

            migrationBuilder.DeleteData(
                table: "StockBalances",
                keyColumn: "Id",
                keyValue: 11L);

            migrationBuilder.DeleteData(
                table: "StockBalances",
                keyColumn: "Id",
                keyValue: 12L);

            migrationBuilder.DeleteData(
                table: "StockBalances",
                keyColumn: "Id",
                keyValue: 13L);

            migrationBuilder.DeleteData(
                table: "StockBalances",
                keyColumn: "Id",
                keyValue: 14L);

            migrationBuilder.DeleteData(
                table: "StockBalances",
                keyColumn: "Id",
                keyValue: 15L);

            migrationBuilder.DeleteData(
                table: "StockBalances",
                keyColumn: "Id",
                keyValue: 16L);

            migrationBuilder.DeleteData(
                table: "StockBalances",
                keyColumn: "Id",
                keyValue: 17L);

            migrationBuilder.DeleteData(
                table: "StockBalances",
                keyColumn: "Id",
                keyValue: 18L);

            migrationBuilder.DeleteData(
                table: "StockBalances",
                keyColumn: "Id",
                keyValue: 19L);

            migrationBuilder.DeleteData(
                table: "StockBalances",
                keyColumn: "Id",
                keyValue: 20L);

            migrationBuilder.DeleteData(
                table: "StockBalances",
                keyColumn: "Id",
                keyValue: 21L);

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 1001L);

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 1002L);

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 1003L);

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 1004L);

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 1005L);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Documents",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Documents",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Documents",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Warehouses",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Customer",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Warehouses",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Warehouses",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Customer",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Customer",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(300)",
                oldMaxLength: 300);

            migrationBuilder.AlterColumn<string>(
                name: "INN",
                table: "Customer",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Customer",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ContactPerson",
                table: "Customer",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);
        }
    }
}
