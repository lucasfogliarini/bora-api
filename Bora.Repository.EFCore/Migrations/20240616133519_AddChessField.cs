using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bora.Repository.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class AddChessField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Chess",
                table: "Account",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Chess",
                table: "Account");
        }
    }
}
