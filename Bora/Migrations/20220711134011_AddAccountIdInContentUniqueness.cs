using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bora.Migrations
{
    public partial class AddAccountIdInContentUniqueness : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Content_Collection_Key",
                table: "Content");

            migrationBuilder.CreateIndex(
                name: "IX_Content_Collection_Key_AccountId",
                table: "Content",
                columns: new[] { "Collection", "Key", "AccountId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Content_Collection_Key_AccountId",
                table: "Content");

            migrationBuilder.CreateIndex(
                name: "IX_Content_Collection_Key",
                table: "Content",
                columns: new[] { "Collection", "Key" },
                unique: true);
        }
    }
}
