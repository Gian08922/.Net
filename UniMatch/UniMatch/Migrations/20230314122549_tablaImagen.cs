using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniMatch.Migrations
{
    public partial class tablaImagen : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Imagen",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "Imagen",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Archivo = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    UsuarioId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Imagen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Imagen_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Imagen_UsuarioId",
                table: "Imagen",
                column: "UsuarioId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Imagen");

            migrationBuilder.AddColumn<byte[]>(
                name: "Imagen",
                table: "AspNetUsers",
                type: "varbinary(max)",
                nullable: true);
        }
    }
}
