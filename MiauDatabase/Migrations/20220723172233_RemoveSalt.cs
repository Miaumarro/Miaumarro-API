using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiauDatabase.Migrations;

public partial class RemoveSalt : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "salt",
            table: "users");

        migrationBuilder.DropColumn(
            name: "salted_password",
            table: "users");

        migrationBuilder.AddColumn<string>(
            name: "hashed_password",
            table: "users",
            type: "TEXT",
            maxLength: 60,
            nullable: false,
            defaultValue: "");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "hashed_password",
            table: "users");

        migrationBuilder.AddColumn<string>(
            name: "salt",
            table: "users",
            type: "TEXT",
            maxLength: 50,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "salted_password",
            table: "users",
            type: "TEXT",
            maxLength: 100,
            nullable: false,
            defaultValue: "");
    }
}
