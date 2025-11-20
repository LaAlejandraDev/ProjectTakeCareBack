using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectTakeCareBack.Migrations
{
    /// <inheritdoc />
    public partial class Citas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DisponibilidadId",
                table: "Citas",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdDisponibilidad",
                table: "Citas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "PsicologoDisponibilidad",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdPsicologo = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CupoMaximo = table.Column<int>(type: "int", nullable: false),
                    CitasAgendadas = table.Column<int>(type: "int", nullable: false),
                    PsicologoId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PsicologoDisponibilidad", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PsicologoDisponibilidad_Psicologos_PsicologoId",
                        column: x => x.PsicologoId,
                        principalTable: "Psicologos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Citas_DisponibilidadId",
                table: "Citas",
                column: "DisponibilidadId");

            migrationBuilder.CreateIndex(
                name: "IX_PsicologoDisponibilidad_PsicologoId",
                table: "PsicologoDisponibilidad",
                column: "PsicologoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Citas_PsicologoDisponibilidad_DisponibilidadId",
                table: "Citas",
                column: "DisponibilidadId",
                principalTable: "PsicologoDisponibilidad",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Citas_PsicologoDisponibilidad_DisponibilidadId",
                table: "Citas");

            migrationBuilder.DropTable(
                name: "PsicologoDisponibilidad");

            migrationBuilder.DropIndex(
                name: "IX_Citas_DisponibilidadId",
                table: "Citas");

            migrationBuilder.DropColumn(
                name: "DisponibilidadId",
                table: "Citas");

            migrationBuilder.DropColumn(
                name: "IdDisponibilidad",
                table: "Citas");
        }
    }
}
