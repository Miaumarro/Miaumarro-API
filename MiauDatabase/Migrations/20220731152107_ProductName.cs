using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiauDatabase.Migrations;

public partial class ProductName : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "name",
            table: "products",
            type: "TEXT",
            maxLength: 50,
            nullable: false,
            defaultValue: "");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "name",
            table: "products");
    }
}
