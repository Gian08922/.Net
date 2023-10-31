using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniMatch.Migrations
{
    public partial class tablaMatch : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Aficiones",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Match",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    User1Id = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    User2Id = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Like = table.Column<bool>(type: "bit", nullable: false),
                    IsMatch = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Match", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Match_AspNetUsers_User1Id",
                        column: x => x.User1Id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Match_AspNetUsers_User2Id",
                        column: x => x.User2Id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Match_User1Id",
                table: "Match",
                column: "User1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Match_User2Id",
                table: "Match",
                column: "User2Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Match");

            migrationBuilder.DropColumn(
                name: "Aficiones",
                table: "AspNetUsers");
        }
    }
}
