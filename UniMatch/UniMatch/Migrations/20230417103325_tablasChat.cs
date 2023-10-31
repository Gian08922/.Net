using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniMatch.Migrations
{
    public partial class tablasChat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChatSala",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Fecha = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatSala", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChatUser",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Usuario1Id = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Usuario2Id = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ChatId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatUser", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatUser_AspNetUsers_Usuario1Id",
                        column: x => x.Usuario1Id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ChatUser_AspNetUsers_Usuario2Id",
                        column: x => x.Usuario2Id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ChatUser_ChatSala_ChatId",
                        column: x => x.ChatId,
                        principalTable: "ChatSala",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Mensaje",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsuarioId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ChatId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Texto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fecha = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mensaje", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Mensaje_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Mensaje_ChatSala_ChatId",
                        column: x => x.ChatId,
                        principalTable: "ChatSala",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatUser_ChatId",
                table: "ChatUser",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatUser_Usuario1Id",
                table: "ChatUser",
                column: "Usuario1Id");

            migrationBuilder.CreateIndex(
                name: "IX_ChatUser_Usuario2Id",
                table: "ChatUser",
                column: "Usuario2Id");

            migrationBuilder.CreateIndex(
                name: "IX_Mensaje_ChatId",
                table: "Mensaje",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_Mensaje_UsuarioId",
                table: "Mensaje",
                column: "UsuarioId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatUser");

            migrationBuilder.DropTable(
                name: "Mensaje");

            migrationBuilder.DropTable(
                name: "ChatSala");
        }
    }
}
