using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shopipy.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDiscounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_OrderDiscounts_OrderId",
                table: "OrderDiscounts",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDiscounts_OrderItemId",
                table: "OrderDiscounts",
                column: "OrderItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDiscounts_OrderItems_OrderItemId",
                table: "OrderDiscounts",
                column: "OrderItemId",
                principalTable: "OrderItems",
                principalColumn: "OrderItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDiscounts_Orders_OrderId",
                table: "OrderDiscounts",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "OrderId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDiscounts_OrderItems_OrderItemId",
                table: "OrderDiscounts");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDiscounts_Orders_OrderId",
                table: "OrderDiscounts");

            migrationBuilder.DropIndex(
                name: "IX_OrderDiscounts_OrderId",
                table: "OrderDiscounts");

            migrationBuilder.DropIndex(
                name: "IX_OrderDiscounts_OrderItemId",
                table: "OrderDiscounts");
        }
    }
}
