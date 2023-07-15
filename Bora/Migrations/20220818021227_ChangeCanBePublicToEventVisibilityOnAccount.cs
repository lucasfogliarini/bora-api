using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bora.Migrations
{
    public partial class ChangeCanBePublicToEventVisibilityOnAccount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PartnerCallsCanBePublic",
                table: "Account");

            migrationBuilder.AddColumn<int>(
                name: "EventVisibility",
                table: "Account",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventVisibility",
                table: "Account");

            migrationBuilder.AddColumn<bool>(
                name: "PartnerCallsCanBePublic",
                table: "Account",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }
    }
}
