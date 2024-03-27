using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bora.Migrations
{
    public partial class AddOnlySelfOrganizerInAccount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "OnlySelfOrganizer",
                table: "Account",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OnlySelfOrganizer",
                table: "Account");
        }
    }
}
