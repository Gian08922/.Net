using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniMatch.Migrations
{
    public partial class camposEditarPerfil : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EdadMax",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EdadMin",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UniversidadBusqueda",
                table: "AspNetUsers",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EdadMax",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "EdadMin",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UniversidadBusqueda",
                table: "AspNetUsers");
        }
    }
}
