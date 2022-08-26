using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiauDatabase.Migrations
{
    public partial class Correcao_WishList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_product_reviews_users_user_id",
                table: "product_reviews");

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
                newName: "wish_lists");

            migrationBuilder.RenameIndex(
                name: "ix_wishlist_user_id",
                table: "wish_lists",
                newName: "ix_wish_lists_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_wishlist_product_id",
                table: "wish_lists",
                newName: "ix_wish_lists_product_id");

            migrationBuilder.AlterTable(
                name: "product_reviews",
                comment: "Represents a product review.",
                oldComment: "Represents a product image.");

            migrationBuilder.AlterColumn<int>(
                name: "user_id",
                table: "product_reviews",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "pk_wish_lists",
                table: "wish_lists",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_product_reviews_users_user_id",
                table: "product_reviews",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_wish_lists_products_product_id",
                table: "wish_lists",
                column: "product_id",
                principalTable: "products",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_wish_lists_users_user_id",
                table: "wish_lists",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_product_reviews_users_user_id",
                table: "product_reviews");

            migrationBuilder.DropForeignKey(
                name: "fk_wish_lists_products_product_id",
                table: "wish_lists");

            migrationBuilder.DropForeignKey(
                name: "fk_wish_lists_users_user_id",
                table: "wish_lists");

            migrationBuilder.DropPrimaryKey(
                name: "pk_wish_lists",
                table: "wish_lists");

            migrationBuilder.RenameTable(
                name: "wish_lists",
                newName: "wishlist");

            migrationBuilder.RenameIndex(
                name: "ix_wish_lists_user_id",
                table: "wishlist",
                newName: "ix_wishlist_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_wish_lists_product_id",
                table: "wishlist",
                newName: "ix_wishlist_product_id");

            migrationBuilder.AlterTable(
                name: "product_reviews",
                comment: "Represents a product image.",
                oldComment: "Represents a product review.");

            migrationBuilder.AlterColumn<int>(
                name: "user_id",
                table: "product_reviews",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddPrimaryKey(
                name: "pk_wishlist",
                table: "wishlist",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_product_reviews_users_user_id",
                table: "product_reviews",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id");

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
}
