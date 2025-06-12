using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APiTurboSetup.Migrations
{
    /// <inheritdoc />
    public partial class AddEnderecoTableIdetificacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Identificacao",
                table: "Enderecos",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Identificacao",
                table: "Enderecos");
        }
    }
}
