using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Shopipy.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveComposite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderPayments",
                table: "OrderPayments");

            migrationBuilder.AlterColumn<int>(
                name: "PaymentId",
                table: "OrderPayments",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderPayments",
                table: "OrderPayments",
                column: "PaymentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderPayments",
                table: "OrderPayments");

            migrationBuilder.AlterColumn<int>(
                name: "PaymentId",
                table: "OrderPayments",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderPayments",
                table: "OrderPayments",
                columns: new[] { "PaymentId", "BusinessId", "OrderId" });
        }
    }
}
