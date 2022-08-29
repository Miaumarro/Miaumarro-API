using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiauDatabase.Migrations;

public partial class RenameWishlist : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_wishlist_products_product_id",
            table: "wishlist");

        migrationBuilder.DropForeignKey(
            name: "fk_wishlist_users_user_id",
            table: "wishlist");

        migrationBuilder.DropPrimaryKey(
            name: "pk_wishlist",
            table: "wishlist");

        migrationBuilder.RenameTable(
            name: "wishlist",
            newName: "wishlists");

        migrationBuilder.RenameIndex(
            name: "ix_wishlist_user_id",
            table: "wishlists",
            newName: "ix_wishlists_user_id");

        migrationBuilder.RenameIndex(
            name: "ix_wishlist_product_id",
            table: "wishlists",
            newName: "ix_wishlists_product_id");

        migrationBuilder.AddPrimaryKey(
            name: "pk_wishlists",
            table: "wishlists",
            column: "id");

        migrationBuilder.AddForeignKey(
            name: "fk_wishlists_products_product_id",
            table: "wishlists",
            column: "product_id",
            principalTable: "products",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "fk_wishlists_users_user_id",
            table: "wishlists",
            column: "user_id",
            principalTable: "users",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_wishlists_products_product_id",
            table: "wishlists");

        migrationBuilder.DropForeignKey(
            name: "fk_wishlists_users_user_id",
            table: "wishlists");

        migrationBuilder.DropPrimaryKey(
            name: "pk_wishlists",
            table: "wishlists");

        migrationBuilder.RenameTable(
            name: "wishlists",
            newName: "wishlist");

        migrationBuilder.RenameIndex(
            name: "ix_wishlists_user_id",
            table: "wishlist",
            newName: "ix_wishlist_user_id");

        migrationBuilder.RenameIndex(
            name: "ix_wishlists_product_id",
            table: "wishlist",
            newName: "ix_wishlist_product_id");

        migrationBuilder.AddPrimaryKey(
            name: "pk_wishlist",
            table: "wishlist",
            column: "id");

        migrationBuilder.AddForeignKey(
            name: "fk_wishlist_products_product_id",
            table: "wishlist",
            column: "product_id",
            principalTable: "products",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "fk_wishlist_users_user_id",
            table: "wishlist",
            column: "user_id",
            principalTable: "users",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);
    }
}
