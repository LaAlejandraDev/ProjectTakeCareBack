using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectTakeCareBack.Migrations
{
    /// <inheritdoc />
    public partial class PacientesV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaNacimiento",
                table: "Pacientes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "FechaNacimiento",
                table: "Pacientes",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }
    }
}
