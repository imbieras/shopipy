using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shopipy.ApiService.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserState",
                table: "AspNetUsers",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserState",
                table: "AspNetUsers");
        }
    }
}
