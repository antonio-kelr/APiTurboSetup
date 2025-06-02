using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APiTurboSetup.Migrations
{
    /// <inheritdoc />
    public partial class AumentaTamanhoDescricao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Descricao",
                table: "Produtos",
                type: "character varying(900)",
                maxLength: 900,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Descricao",
                table: "Produtos",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(900)",
                oldMaxLength: 900,
                oldNullable: true);
        }
    }
}
