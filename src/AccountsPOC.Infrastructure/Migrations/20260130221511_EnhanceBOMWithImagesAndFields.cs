using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountsPOC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EnhanceBOMWithImagesAndFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsOptional",
                table: "BOMComponents",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "LineNumber",
                table: "BOMComponents",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "BOMComponents",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ScrapPercentage",
                table: "BOMComponents",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BOMImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BillOfMaterialId = table.Column<int>(type: "INTEGER", nullable: false),
                    ImageUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    ImageData = table.Column<string>(type: "TEXT", nullable: true),
                    Caption = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    IsPrimaryImage = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BOMImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BOMImages_BillOfMaterials_BillOfMaterialId",
                        column: x => x.BillOfMaterialId,
                        principalTable: "BillOfMaterials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BOMImages_BillOfMaterialId",
                table: "BOMImages",
                column: "BillOfMaterialId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BOMImages");

            migrationBuilder.DropColumn(
                name: "IsOptional",
                table: "BOMComponents");

            migrationBuilder.DropColumn(
                name: "LineNumber",
                table: "BOMComponents");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "BOMComponents");

            migrationBuilder.DropColumn(
                name: "ScrapPercentage",
                table: "BOMComponents");
        }
    }
}
