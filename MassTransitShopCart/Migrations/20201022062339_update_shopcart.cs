using Microsoft.EntityFrameworkCore.Migrations;

namespace MassTransitShopCart.Migrations
{
    public partial class update_shopcart : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "ShopCarts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserName",
                table: "ShopCarts");
        }
    }
}
