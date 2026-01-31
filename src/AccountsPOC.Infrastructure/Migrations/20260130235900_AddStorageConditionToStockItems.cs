using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountsPOC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStorageConditionToStockItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StorageCondition",
                table: "StockItems",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StorageCondition",
                table: "StockItems");
        }
    }
}
