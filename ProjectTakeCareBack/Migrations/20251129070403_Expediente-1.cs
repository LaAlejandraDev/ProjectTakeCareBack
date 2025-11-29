using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectTakeCareBack.Migrations
{
    /// <inheritdoc />
    public partial class Expediente1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExpedienteId",
                table: "DiarioEmocional",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExpedienteId",
                table: "Citas",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Expediente",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PacienteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expediente", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Expediente_Pacientes_PacienteId",
                        column: x => x.PacienteId,
                        principalTable: "Pacientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DiarioEmocional_ExpedienteId",
                table: "DiarioEmocional",
                column: "ExpedienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Citas_ExpedienteId",
                table: "Citas",
                column: "ExpedienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Expediente_PacienteId",
                table: "Expediente",
                column: "PacienteId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Citas_Expediente_ExpedienteId",
                table: "Citas",
                column: "ExpedienteId",
                principalTable: "Expediente",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DiarioEmocional_Expediente_ExpedienteId",
                table: "DiarioEmocional",
                column: "ExpedienteId",
                principalTable: "Expediente",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Citas_Expediente_ExpedienteId",
                table: "Citas");

            migrationBuilder.DropForeignKey(
                name: "FK_DiarioEmocional_Expediente_ExpedienteId",
                table: "DiarioEmocional");

            migrationBuilder.DropTable(
                name: "Expediente");

            migrationBuilder.DropIndex(
                name: "IX_DiarioEmocional_ExpedienteId",
                table: "DiarioEmocional");

            migrationBuilder.DropIndex(
                name: "IX_Citas_ExpedienteId",
                table: "Citas");

            migrationBuilder.DropColumn(
                name: "ExpedienteId",
                table: "DiarioEmocional");

            migrationBuilder.DropColumn(
                name: "ExpedienteId",
                table: "Citas");
        }
    }
}
