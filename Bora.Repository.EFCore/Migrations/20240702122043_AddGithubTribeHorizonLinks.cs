using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bora.Repository.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class AddGithubTribeHorizonLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Github",
                table: "Account",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HorizonMeta",
                table: "Account",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tribe",
                table: "Account",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Github",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "HorizonMeta",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "Tribe",
                table: "Account");
        }
    }
}
