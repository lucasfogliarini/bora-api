using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bora.Repository.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class AddPlaceInLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Place",
                table: "Location",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Place",
                table: "Location");
        }
    }
}
