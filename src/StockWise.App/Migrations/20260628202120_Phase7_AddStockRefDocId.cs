using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockWise.App.Migrations
{
    /// <inheritdoc />
    public partial class Phase7_AddStockRefDocId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StockRefDocId",
                table: "Documents",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StockRefDocId",
                table: "Documents");
        }
    }
}
