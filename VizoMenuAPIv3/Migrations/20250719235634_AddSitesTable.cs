using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VizoMenuAPIv3.Migrations
{
    /// <inheritdoc />
    public partial class AddSitesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sites_Venues_VenueId",
                table: "Sites");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Sites",
                table: "Sites");

            migrationBuilder.RenameTable(
                name: "Sites",
                newName: "VenueSites");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "VenueSites",
                newName: "SiteName");

            migrationBuilder.RenameIndex(
                name: "IX_Sites_VenueId",
                table: "VenueSites",
                newName: "IX_VenueSites_VenueId");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "VenueSites",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EnteredById",
                table: "VenueSites",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "EnteredUTC",
                table: "VenueSites",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "VenueSites",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_VenueSites",
                table: "VenueSites",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VenueSites_Venues_VenueId",
                table: "VenueSites",
                column: "VenueId",
                principalTable: "Venues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VenueSites_Venues_VenueId",
                table: "VenueSites");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VenueSites",
                table: "VenueSites");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "VenueSites");

            migrationBuilder.DropColumn(
                name: "EnteredById",
                table: "VenueSites");

            migrationBuilder.DropColumn(
                name: "EnteredUTC",
                table: "VenueSites");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "VenueSites");

            migrationBuilder.RenameTable(
                name: "VenueSites",
                newName: "Sites");

            migrationBuilder.RenameColumn(
                name: "SiteName",
                table: "Sites",
                newName: "Name");

            migrationBuilder.RenameIndex(
                name: "IX_VenueSites_VenueId",
                table: "Sites",
                newName: "IX_Sites_VenueId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sites",
                table: "Sites",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Sites_Venues_VenueId",
                table: "Sites",
                column: "VenueId",
                principalTable: "Venues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
