using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Data.Migrations
{
    /// <inheritdoc />
    public partial class MechanicAndClientWorkorder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrders_AspNetUsers_UserId",
                table: "WorkOrders");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "WorkOrders",
                newName: "MechanicId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkOrders_UserId",
                table: "WorkOrders",
                newName: "IX_WorkOrders_MechanicId");

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "WorkOrders",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_ClientId",
                table: "WorkOrders",
                column: "ClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrders_AspNetUsers_ClientId",
                table: "WorkOrders",
                column: "ClientId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrders_AspNetUsers_MechanicId",
                table: "WorkOrders",
                column: "MechanicId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrders_AspNetUsers_ClientId",
                table: "WorkOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrders_AspNetUsers_MechanicId",
                table: "WorkOrders");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrders_ClientId",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "WorkOrders");

            migrationBuilder.RenameColumn(
                name: "MechanicId",
                table: "WorkOrders",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkOrders_MechanicId",
                table: "WorkOrders",
                newName: "IX_WorkOrders_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrders_AspNetUsers_UserId",
                table: "WorkOrders",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
