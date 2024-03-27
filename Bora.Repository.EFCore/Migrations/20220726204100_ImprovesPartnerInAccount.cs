using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bora.Migrations
{
    public partial class ImprovesPartnerInAccount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreateOrderEnabled",
                table: "Account",
                newName: "PartnerCommentsEnabled");

            migrationBuilder.RenameColumn(
                name: "CreateEventEnabled",
                table: "Account",
                newName: "IsPartner");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PartnerCommentsEnabled",
                table: "Account",
                newName: "CreateOrderEnabled");

            migrationBuilder.RenameColumn(
                name: "IsPartner",
                table: "Account",
                newName: "CreateEventEnabled");
        }
    }
}
