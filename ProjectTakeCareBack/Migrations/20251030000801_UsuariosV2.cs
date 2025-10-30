using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectTakeCareBack.Migrations
{
    /// <inheritdoc />
    public partial class UsuariosV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Genero",
                table: "Pacientes");

            migrationBuilder.AddColumn<string>(
                name: "Genero",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Genero",
                table: "Usuarios");

            migrationBuilder.AddColumn<string>(
                name: "Genero",
                table: "Pacientes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
