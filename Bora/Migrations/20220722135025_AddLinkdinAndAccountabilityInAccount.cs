using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bora.Migrations
{
    public partial class AddLinkdinAndAccountabilityInAccount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Accountability",
                table: "Account",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Linkedin",
                table: "Account",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Accountability",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "Linkedin",
                table: "Account");
        }
    }
}
