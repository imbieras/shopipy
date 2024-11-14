using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shopipy.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class addNullabelBuisinessIdToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BusinessId",
                table: "AspNetUsers",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_BusinessId",
                table: "AspNetUsers",
                column: "BusinessId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Businesses_BusinessId",
                table: "AspNetUsers",
                column: "BusinessId",
                principalTable: "Businesses",
                principalColumn: "BusinessId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Businesses_BusinessId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_BusinessId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "BusinessId",
                table: "AspNetUsers");
        }
    }
}
