using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountsPOC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDigitalProductAndGiftCardFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDigitalProduct",
                table: "StockItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsGiftCard",
                table: "StockItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDigitalProduct",
                table: "StockItems");

            migrationBuilder.DropColumn(
                name: "IsGiftCard",
                table: "StockItems");
        }
    }
}
