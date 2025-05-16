using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APiTurboSetup.Migrations
{
    /// <inheritdoc />
    public partial class AddCpfGenero_Cadatro : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Cpf",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataNascimento",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Genero",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cpf",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DataNascimento",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Genero",
                table: "Users");
        }
    }
}
