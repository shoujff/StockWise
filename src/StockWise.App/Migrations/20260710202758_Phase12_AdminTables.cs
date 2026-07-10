using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockWise.App.Migrations
{
    /// <inheritdoc />
    public partial class Phase12_AdminTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditLog_Users_UserId",
                table: "AuditLog");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemPrice_PriceGroup_PriceGroupId",
                table: "ItemPrice");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PriceGroup",
                table: "PriceGroup");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AuditLog",
                table: "AuditLog");

            migrationBuilder.RenameTable(
                name: "PriceGroup",
                newName: "PriceGroups");

            migrationBuilder.RenameTable(
                name: "AuditLog",
                newName: "AuditLogs");

            migrationBuilder.RenameIndex(
                name: "IX_AuditLog_UserId",
                table: "AuditLogs",
                newName: "IX_AuditLogs_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PriceGroups",
                table: "PriceGroups",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuditLogs",
                table: "AuditLogs",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogs_Users_UserId",
                table: "AuditLogs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemPrice_PriceGroups_PriceGroupId",
                table: "ItemPrice",
                column: "PriceGroupId",
                principalTable: "PriceGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogs_Users_UserId",
                table: "AuditLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemPrice_PriceGroups_PriceGroupId",
                table: "ItemPrice");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PriceGroups",
                table: "PriceGroups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AuditLogs",
                table: "AuditLogs");

            migrationBuilder.RenameTable(
                name: "PriceGroups",
                newName: "PriceGroup");

            migrationBuilder.RenameTable(
                name: "AuditLogs",
                newName: "AuditLog");

            migrationBuilder.RenameIndex(
                name: "IX_AuditLogs_UserId",
                table: "AuditLog",
                newName: "IX_AuditLog_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PriceGroup",
                table: "PriceGroup",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuditLog",
                table: "AuditLog",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLog_Users_UserId",
                table: "AuditLog",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemPrice_PriceGroup_PriceGroupId",
                table: "ItemPrice",
                column: "PriceGroupId",
                principalTable: "PriceGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
