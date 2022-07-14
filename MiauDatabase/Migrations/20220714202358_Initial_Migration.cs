using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiauDatabase.Migrations
{
    public partial class Initial_Migration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    price = table.Column<decimal>(type: "TEXT", nullable: false),
                    is_active = table.Column<bool>(type: "INTEGER", nullable: false),
                    amount = table.Column<int>(type: "INTEGER", nullable: false),
                    tags = table.Column<int>(type: "INTEGER", nullable: false),
                    brand = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true),
                    discount = table.Column<decimal>(type: "TEXT", nullable: false),
                    date_added = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_products", x => x.id);
                },
                comment: "Represents a store product.");

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    cpf = table.Column<string>(type: "TEXT", maxLength: 14, nullable: false),
                    name = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    surname = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    email = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    phone = table.Column<string>(type: "TEXT", maxLength: 14, nullable: true),
                    salt = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    salted_password = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    permissions = table.Column<int>(type: "INTEGER", nullable: false),
                    date_added = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                },
                comment: "Represents a user.");

            migrationBuilder.CreateTable(
                name: "product_images",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    fk_product_id = table.Column<int>(type: "INTEGER", nullable: false),
                    file_url = table.Column<string>(type: "TEXT", nullable: false),
                    date_added = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_product_images", x => x.id);
                    table.ForeignKey(
                        name: "fk_product_images_products_fk_product_id",
                        column: x => x.fk_product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Represents a product image.");

            migrationBuilder.CreateTable(
                name: "addresses",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    fk_user_id = table.Column<int>(type: "INTEGER", nullable: false),
                    address = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    number = table.Column<int>(type: "INTEGER", nullable: false),
                    reference = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    complement = table.Column<string>(type: "TEXT", maxLength: 15, nullable: true),
                    neighborhood = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    city = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    state = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    destinatary = table.Column<string>(type: "TEXT", maxLength: 60, nullable: true),
                    cep = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    date_added = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_addresses", x => x.id);
                    table.ForeignKey(
                        name: "fk_addresses_users_fk_user_id",
                        column: x => x.fk_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Represents an address.");

            migrationBuilder.CreateTable(
                name: "pets",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    fk_user_id = table.Column<int>(type: "INTEGER", nullable: false),
                    name = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    type = table.Column<int>(type: "INTEGER", nullable: false),
                    gender = table.Column<int>(type: "INTEGER", nullable: false),
                    breed = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true),
                    image_file_url = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    date_of_birth = table.Column<DateTime>(type: "TEXT", nullable: false),
                    date_added = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pets", x => x.id);
                    table.ForeignKey(
                        name: "fk_pets_users_fk_user_id",
                        column: x => x.fk_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Represents a pet.");

            migrationBuilder.CreateTable(
                name: "product_reviews",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    fk_user_id = table.Column<int>(type: "INTEGER", nullable: true),
                    fk_product_id = table.Column<int>(type: "INTEGER", nullable: false),
                    description = table.Column<string>(type: "TEXT", nullable: false),
                    score = table.Column<int>(type: "INTEGER", nullable: false),
                    date_added = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_product_reviews", x => x.id);
                    table.ForeignKey(
                        name: "fk_product_reviews_products_fk_product_id",
                        column: x => x.fk_product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_product_reviews_users_fk_user_id",
                        column: x => x.fk_user_id,
                        principalTable: "users",
                        principalColumn: "id");
                },
                comment: "Represents a product image.");

            migrationBuilder.CreateTable(
                name: "purchases",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    fk_user_id = table.Column<int>(type: "INTEGER", nullable: false),
                    coupon = table.Column<string>(type: "TEXT", nullable: true),
                    status = table.Column<int>(type: "INTEGER", nullable: false),
                    date_added = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_purchases", x => x.id);
                    table.ForeignKey(
                        name: "fk_purchases_users_fk_user_id",
                        column: x => x.fk_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Represents a purchase.");

            migrationBuilder.CreateTable(
                name: "wishlist",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    fk_user_id = table.Column<int>(type: "INTEGER", nullable: false),
                    fk_product_id = table.Column<int>(type: "INTEGER", nullable: false),
                    date_added = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_wishlist", x => x.id);
                    table.ForeignKey(
                        name: "fk_wishlist_products_fk_product_id",
                        column: x => x.fk_product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_wishlist_users_fk_user_id",
                        column: x => x.fk_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Represents a wishlist item.");

            migrationBuilder.CreateTable(
                name: "appointments",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    fk_pet_id = table.Column<int>(type: "INTEGER", nullable: false),
                    price = table.Column<decimal>(type: "TEXT", nullable: false),
                    type = table.Column<int>(type: "INTEGER", nullable: false),
                    scheduled_time = table.Column<DateTime>(type: "TEXT", nullable: false),
                    date_added = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_appointments", x => x.id);
                    table.ForeignKey(
                        name: "fk_appointments_pets_fk_pet_id",
                        column: x => x.fk_pet_id,
                        principalTable: "pets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Represents an appointment.");

            migrationBuilder.CreateTable(
                name: "purchased_products",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    fk_product_id = table.Column<int>(type: "INTEGER", nullable: false),
                    fk_purchase_id = table.Column<int>(type: "INTEGER", nullable: false),
                    sale_price = table.Column<decimal>(type: "TEXT", nullable: false),
                    date_added = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_purchased_products", x => x.id);
                    table.ForeignKey(
                        name: "fk_purchased_products_products_fk_product_id",
                        column: x => x.fk_product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_purchased_products_purchases_fk_purchase_id",
                        column: x => x.fk_purchase_id,
                        principalTable: "purchases",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Represents a purchased product.");

            migrationBuilder.CreateIndex(
                name: "ix_addresses_fk_user_id",
                table: "addresses",
                column: "fk_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_appointments_fk_pet_id",
                table: "appointments",
                column: "fk_pet_id");

            migrationBuilder.CreateIndex(
                name: "ix_pets_fk_user_id",
                table: "pets",
                column: "fk_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_product_images_fk_product_id",
                table: "product_images",
                column: "fk_product_id");

            migrationBuilder.CreateIndex(
                name: "ix_product_reviews_fk_product_id",
                table: "product_reviews",
                column: "fk_product_id");

            migrationBuilder.CreateIndex(
                name: "ix_product_reviews_fk_user_id",
                table: "product_reviews",
                column: "fk_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_purchased_products_fk_product_id",
                table: "purchased_products",
                column: "fk_product_id");

            migrationBuilder.CreateIndex(
                name: "ix_purchased_products_fk_purchase_id",
                table: "purchased_products",
                column: "fk_purchase_id");

            migrationBuilder.CreateIndex(
                name: "ix_purchases_fk_user_id",
                table: "purchases",
                column: "fk_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_wishlist_fk_product_id",
                table: "wishlist",
                column: "fk_product_id");

            migrationBuilder.CreateIndex(
                name: "ix_wishlist_fk_user_id",
                table: "wishlist",
                column: "fk_user_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "addresses");

            migrationBuilder.DropTable(
                name: "appointments");

            migrationBuilder.DropTable(
                name: "product_images");

            migrationBuilder.DropTable(
                name: "product_reviews");

            migrationBuilder.DropTable(
                name: "purchased_products");

            migrationBuilder.DropTable(
                name: "wishlist");

            migrationBuilder.DropTable(
                name: "pets");

            migrationBuilder.DropTable(
                name: "purchases");

            migrationBuilder.DropTable(
                name: "products");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
