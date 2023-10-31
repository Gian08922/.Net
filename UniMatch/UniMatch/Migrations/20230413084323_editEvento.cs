using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniMatch.Migrations
{
    public partial class editEvento : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Direccion",
                table: "Evento",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FechaEvento",
                table: "Evento",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Direccion",
                table: "Evento");

            migrationBuilder.DropColumn(
                name: "FechaEvento",
                table: "Evento");
        }
    }
}
