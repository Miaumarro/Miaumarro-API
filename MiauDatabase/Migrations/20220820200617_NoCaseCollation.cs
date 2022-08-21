using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiauDatabase.Migrations;

public partial class NoCaseCollation : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterDatabase(
            collation: "NOCASE");

        migrationBuilder.AlterTable(
            name: "product_reviews",
            comment: "Represents a product review.",
            oldComment: "Represents a product image.");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterDatabase(
            oldCollation: "NOCASE");

        migrationBuilder.AlterTable(
            name: "product_reviews",
            comment: "Represents a product image.",
            oldComment: "Represents a product review.");
    }
}
