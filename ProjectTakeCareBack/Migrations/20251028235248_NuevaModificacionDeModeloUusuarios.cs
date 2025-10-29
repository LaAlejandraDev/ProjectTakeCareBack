using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectTakeCareBack.Migrations
{
    /// <inheritdoc />
    public partial class NuevaModificacionDeModeloUusuarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ContrasenaHash",
                table: "Usuarios",
                newName: "Contrasena");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Contrasena",
                table: "Usuarios",
                newName: "ContrasenaHash");
        }
    }
}
