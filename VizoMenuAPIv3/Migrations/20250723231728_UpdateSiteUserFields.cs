using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VizoMenuAPIv3.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSiteUserFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActiveChangedById",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "EnteredById",
                table: "Sites");

            migrationBuilder.AddColumn<string>(
                name: "ActiveChangedBy",
                table: "Sites",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EnteredBy",
                table: "Sites",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActiveChangedBy",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "EnteredBy",
                table: "Sites");

            migrationBuilder.AddColumn<Guid>(
                name: "ActiveChangedById",
                table: "Sites",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EnteredById",
                table: "Sites",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
