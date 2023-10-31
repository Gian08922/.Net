using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniMatch.Migrations
{
    public partial class interesSexual : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OrientSex",
                table: "AspNetUsers",
                newName: "Interes");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Interes",
                table: "AspNetUsers",
                newName: "OrientSex");
        }
    }
}
