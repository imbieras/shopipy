using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shopipy.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class adjustedAppointmentModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // First, add a new temporary column
            migrationBuilder.AddColumn<Guid>(
                name: "EmployeeId_New",
                table: "Appointments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid());

            // Drop the old column
            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "Appointments");

            // Rename the new column to the original name
            migrationBuilder.RenameColumn(
                name: "EmployeeId_New",
                table: "Appointments",
                newName: "EmployeeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // First, add a temporary integer column
            migrationBuilder.AddColumn<int>(
                name: "EmployeeId_Old",
                table: "Appointments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            // Drop the UUID column
            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "Appointments");

            // Rename the temporary column back
            migrationBuilder.RenameColumn(
                name: "EmployeeId_Old",
                table: "Appointments",
                newName: "EmployeeId");
        }
    }
}
