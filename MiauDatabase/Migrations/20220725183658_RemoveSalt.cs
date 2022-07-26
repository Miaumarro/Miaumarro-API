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

        migrationBuilder.CreateIndex(
            name: "ix_users_cpf",
            table: "users",
            column: "cpf",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_users_email",
            table: "users",
            column: "email",
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "ix_users_cpf",
            table: "users");

        migrationBuilder.DropIndex(
            name: "ix_users_email",
            table: "users");

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
