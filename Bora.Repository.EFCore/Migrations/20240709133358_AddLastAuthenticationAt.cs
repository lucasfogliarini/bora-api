using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bora.Repository.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class AddLastAuthenticationAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastAuthenticationAt",
                table: "Account",
                type: "datetimeoffset",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastAuthenticationAt",
                table: "Account");
        }
    }
}
