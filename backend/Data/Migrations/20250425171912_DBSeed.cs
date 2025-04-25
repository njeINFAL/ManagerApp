using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace backend.Data.Migrations
{
    /// <inheritdoc />
    public partial class DBSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "PartsCategories",
                columns: new[] { "PartsCategoryId", "PartsCategoryName" },
                values: new object[] { 1, "Fékrendszer" });

            migrationBuilder.InsertData(
                table: "WorkOrders",
                columns: new[] { "WorkOrderId", "AppointmentTime", "CarId", "ClientId", "CreatedAt", "IsActive", "MechanicId", "Notes" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 4, 29, 14, 0, 0, 0, DateTimeKind.Unspecified), null, null, new DateTime(2025, 4, 25, 16, 30, 0, 0, DateTimeKind.Unspecified), true, null, null },
                    { 2, new DateTime(2025, 4, 29, 13, 0, 0, 0, DateTimeKind.Unspecified), null, null, new DateTime(2025, 4, 25, 16, 30, 10, 0, DateTimeKind.Unspecified), false, null, "TÖRÖLVE" },
                    { 3, new DateTime(2025, 4, 30, 10, 0, 0, 0, DateTimeKind.Unspecified), null, null, new DateTime(2025, 4, 25, 16, 30, 0, 0, DateTimeKind.Unspecified), true, null, null }
                });

            migrationBuilder.InsertData(
                table: "PartItems",
                columns: new[] { "PartItemId", "PartItemName", "PartsCategoryId" },
                values: new object[,]
                {
                    { 1, "Fék tárcsa", 1 },
                    { 2, "Fék dob", 1 },
                    { 3, "Első fékpofa", 1 },
                    { 4, "Hátsó fékpofa", 1 },
                    { 5, "ABS gyűrű", 1 },
                    { 6, "Első fékbetét", 1 },
                    { 7, "Hátső fékbetét", 1 },
                    { 8, "Fékkar", 1 },
                    { 9, "Féktárcsa csavar", 1 },
                    { 10, "Féklopás jelző", 1 },
                    { 11, "Komplett fékrendszer", 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PartItems",
                keyColumn: "PartItemId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "PartItems",
                keyColumn: "PartItemId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "PartItems",
                keyColumn: "PartItemId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "PartItems",
                keyColumn: "PartItemId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "PartItems",
                keyColumn: "PartItemId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "PartItems",
                keyColumn: "PartItemId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "PartItems",
                keyColumn: "PartItemId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "PartItems",
                keyColumn: "PartItemId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "PartItems",
                keyColumn: "PartItemId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "PartItems",
                keyColumn: "PartItemId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "PartItems",
                keyColumn: "PartItemId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "WorkOrders",
                keyColumn: "WorkOrderId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "WorkOrders",
                keyColumn: "WorkOrderId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "WorkOrders",
                keyColumn: "WorkOrderId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "PartsCategories",
                keyColumn: "PartsCategoryId",
                keyValue: 1);
        }
    }
}
