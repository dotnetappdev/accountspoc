using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountsPOC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBankAccountAndWarehouseEnhancements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Bins",
                table: "Warehouses",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HeightLevels",
                table: "Warehouses",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsTemperatureControlled",
                table: "Warehouses",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PickingSequence",
                table: "Warehouses",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecurityLevel",
                table: "Warehouses",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TemperatureRange",
                table: "Warehouses",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Zones",
                table: "Warehouses",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Region",
                table: "BankAccounts",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "VatTaxRate",
                table: "BankAccounts",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bins",
                table: "Warehouses");

            migrationBuilder.DropColumn(
                name: "HeightLevels",
                table: "Warehouses");

            migrationBuilder.DropColumn(
                name: "IsTemperatureControlled",
                table: "Warehouses");

            migrationBuilder.DropColumn(
                name: "PickingSequence",
                table: "Warehouses");

            migrationBuilder.DropColumn(
                name: "SecurityLevel",
                table: "Warehouses");

            migrationBuilder.DropColumn(
                name: "TemperatureRange",
                table: "Warehouses");

            migrationBuilder.DropColumn(
                name: "Zones",
                table: "Warehouses");

            migrationBuilder.DropColumn(
                name: "Region",
                table: "BankAccounts");

            migrationBuilder.DropColumn(
                name: "VatTaxRate",
                table: "BankAccounts");
        }
    }
}
