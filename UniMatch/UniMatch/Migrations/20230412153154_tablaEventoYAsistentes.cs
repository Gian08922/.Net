using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniMatch.Migrations
{
    public partial class tablaEventoYAsistentes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Evento",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreadorEventoId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumMaxAsistentes = table.Column<int>(type: "int", nullable: false),
                    NumAsistentes = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Evento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Evento_AspNetUsers_CreadorEventoId",
                        column: x => x.CreadorEventoId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Asistentes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AsistenteId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    EventoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Asistentes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Asistentes_AspNetUsers_AsistenteId",
                        column: x => x.AsistenteId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Asistentes_Evento_EventoId",
                        column: x => x.EventoId,
                        principalTable: "Evento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Asistentes_AsistenteId",
                table: "Asistentes",
                column: "AsistenteId");

            migrationBuilder.CreateIndex(
                name: "IX_Asistentes_EventoId",
                table: "Asistentes",
                column: "EventoId");

            migrationBuilder.CreateIndex(
                name: "IX_Evento_CreadorEventoId",
                table: "Evento",
                column: "CreadorEventoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Asistentes");

            migrationBuilder.DropTable(
                name: "Evento");
        }
    }
}
