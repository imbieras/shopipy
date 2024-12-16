using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shopipy.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class changed_services_model : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServiceBasePrice",
                table: "Services");

            migrationBuilder.RenameColumn(
                name: "ServiceServiceCharge",
                table: "Services",
                newName: "ServicePrice");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ServicePrice",
                table: "Services",
                newName: "ServiceServiceCharge");

            migrationBuilder.AddColumn<decimal>(
                name: "ServiceBasePrice",
                table: "Services",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
