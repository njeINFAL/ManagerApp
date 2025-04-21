using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddResponsibleUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResponsibleUserId",
                table: "WorkOrderServicess",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderServicess_ResponsibleUserId",
                table: "WorkOrderServicess",
                column: "ResponsibleUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrderServicess_AspNetUsers_ResponsibleUserId",
                table: "WorkOrderServicess",
                column: "ResponsibleUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrderServicess_AspNetUsers_ResponsibleUserId",
                table: "WorkOrderServicess");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrderServicess_ResponsibleUserId",
                table: "WorkOrderServicess");

            migrationBuilder.DropColumn(
                name: "ResponsibleUserId",
                table: "WorkOrderServicess");
        }
    }
}
