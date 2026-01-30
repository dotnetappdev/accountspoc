using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountsPOC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSiteVisitsEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SiteVisitSignOffs_WorkOrders_WorkOrderId",
                table: "SiteVisitSignOffs");

            migrationBuilder.AddColumn<int>(
                name: "SiteVisitId",
                table: "WorkOrders",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "WorkOrderId",
                table: "SiteVisitSignOffs",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<int>(
                name: "SiteVisitId",
                table: "SiteVisitSignOffs",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SiteVisits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TenantId = table.Column<int>(type: "INTEGER", nullable: false),
                    VisitNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    VisitDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ScheduledStartTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ScheduledEndTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ActualStartTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ActualEndTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    SiteAddress = table.Column<string>(type: "TEXT", nullable: true),
                    SiteCity = table.Column<string>(type: "TEXT", nullable: true),
                    SitePostCode = table.Column<string>(type: "TEXT", nullable: true),
                    SiteCountry = table.Column<string>(type: "TEXT", nullable: true),
                    SiteContactName = table.Column<string>(type: "TEXT", nullable: true),
                    SiteContactPhone = table.Column<string>(type: "TEXT", nullable: true),
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: true),
                    CustomerName = table.Column<string>(type: "TEXT", nullable: true),
                    VisitType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Purpose = table.Column<string>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    AssignedToUserId = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteVisits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SiteVisits_AspNetUsers_AssignedToUserId",
                        column: x => x.AssignedToUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SiteVisits_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_SiteVisitId",
                table: "WorkOrders",
                column: "SiteVisitId");

            migrationBuilder.CreateIndex(
                name: "IX_SiteVisitSignOffs_SiteVisitId",
                table: "SiteVisitSignOffs",
                column: "SiteVisitId");

            migrationBuilder.CreateIndex(
                name: "IX_SiteVisits_AssignedToUserId",
                table: "SiteVisits",
                column: "AssignedToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SiteVisits_CustomerId",
                table: "SiteVisits",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_SiteVisits_TenantId",
                table: "SiteVisits",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SiteVisits_VisitNumber",
                table: "SiteVisits",
                column: "VisitNumber",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SiteVisitSignOffs_SiteVisits_SiteVisitId",
                table: "SiteVisitSignOffs",
                column: "SiteVisitId",
                principalTable: "SiteVisits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SiteVisitSignOffs_WorkOrders_WorkOrderId",
                table: "SiteVisitSignOffs",
                column: "WorkOrderId",
                principalTable: "WorkOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrders_SiteVisits_SiteVisitId",
                table: "WorkOrders",
                column: "SiteVisitId",
                principalTable: "SiteVisits",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SiteVisitSignOffs_SiteVisits_SiteVisitId",
                table: "SiteVisitSignOffs");

            migrationBuilder.DropForeignKey(
                name: "FK_SiteVisitSignOffs_WorkOrders_WorkOrderId",
                table: "SiteVisitSignOffs");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrders_SiteVisits_SiteVisitId",
                table: "WorkOrders");

            migrationBuilder.DropTable(
                name: "SiteVisits");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrders_SiteVisitId",
                table: "WorkOrders");

            migrationBuilder.DropIndex(
                name: "IX_SiteVisitSignOffs_SiteVisitId",
                table: "SiteVisitSignOffs");

            migrationBuilder.DropColumn(
                name: "SiteVisitId",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "SiteVisitId",
                table: "SiteVisitSignOffs");

            migrationBuilder.AlterColumn<int>(
                name: "WorkOrderId",
                table: "SiteVisitSignOffs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SiteVisitSignOffs_WorkOrders_WorkOrderId",
                table: "SiteVisitSignOffs",
                column: "WorkOrderId",
                principalTable: "WorkOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
