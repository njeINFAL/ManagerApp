using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddParttables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PartOrders",
                columns: table => new
                {
                    PartOrderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    workOrderId = table.Column<int>(type: "int", nullable: false),
                    Requestor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MechanicId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Approver = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartOrders", x => x.PartOrderId);
                    table.ForeignKey(
                        name: "FK_PartOrders_AspNetUsers_MechanicId",
                        column: x => x.MechanicId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PartsCategories",
                columns: table => new
                {
                    PartsCategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PartsCategoryName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartsCategories", x => x.PartsCategoryId);
                });

            migrationBuilder.CreateTable(
                name: "PartItems",
                columns: table => new
                {
                    PartItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PartItemName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PartsCategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartItems", x => x.PartItemId);
                    table.ForeignKey(
                        name: "FK_PartItems_PartsCategories_PartsCategoryId",
                        column: x => x.PartsCategoryId,
                        principalTable: "PartsCategories",
                        principalColumn: "PartsCategoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PartOrderItems",
                columns: table => new
                {
                    PartOrderItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PartItemId = table.Column<int>(type: "int", nullable: false),
                    PartOrderId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartOrderItems", x => x.PartOrderItemId);
                    table.ForeignKey(
                        name: "FK_PartOrderItems_PartItems_PartItemId",
                        column: x => x.PartItemId,
                        principalTable: "PartItems",
                        principalColumn: "PartItemId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PartOrderItems_PartOrders_PartOrderId",
                        column: x => x.PartOrderId,
                        principalTable: "PartOrders",
                        principalColumn: "PartOrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PartItems_PartsCategoryId",
                table: "PartItems",
                column: "PartsCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_PartOrderItems_PartItemId",
                table: "PartOrderItems",
                column: "PartItemId");

            migrationBuilder.CreateIndex(
                name: "IX_PartOrderItems_PartOrderId",
                table: "PartOrderItems",
                column: "PartOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_PartOrders_MechanicId",
                table: "PartOrders",
                column: "MechanicId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PartOrderItems");

            migrationBuilder.DropTable(
                name: "PartItems");

            migrationBuilder.DropTable(
                name: "PartOrders");

            migrationBuilder.DropTable(
                name: "PartsCategories");
        }
    }
}
