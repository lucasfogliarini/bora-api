using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bora.Repository.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class AddResponsabilityArea : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AreaId",
                table: "Responsibility",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ResponsibilityArea",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResponsibilityArea", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Responsibility_AreaId",
                table: "Responsibility",
                column: "AreaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Responsibility_ResponsibilityArea_AreaId",
                table: "Responsibility",
                column: "AreaId",
                principalTable: "ResponsibilityArea",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Responsibility_ResponsibilityArea_AreaId",
                table: "Responsibility");

            migrationBuilder.DropTable(
                name: "ResponsibilityArea");

            migrationBuilder.DropIndex(
                name: "IX_Responsibility_AreaId",
                table: "Responsibility");

            migrationBuilder.DropColumn(
                name: "AreaId",
                table: "Responsibility");
        }
    }
}
