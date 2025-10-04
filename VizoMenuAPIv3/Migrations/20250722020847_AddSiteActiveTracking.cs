using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VizoMenuAPIv3.Migrations
{
    /// <inheritdoc />
    public partial class AddSiteActiveTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VenueSites_Venues_VenueId",
                table: "VenueSites");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VenueSites",
                table: "VenueSites");

            migrationBuilder.RenameTable(
                name: "VenueSites",
                newName: "Sites");

            migrationBuilder.RenameIndex(
                name: "IX_VenueSites_VenueId",
                table: "Sites",
                newName: "IX_Sites_VenueId");

            migrationBuilder.AddColumn<Guid>(
                name: "ActiveChangedById",
                table: "Sites",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ActiveChangedUTC",
                table: "Sites",
                type: "datetime2",
                nullable: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sites_Venues_VenueId",
                table: "Sites");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Sites",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "ActiveChangedById",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "ActiveChangedUTC",
                table: "Sites");

            migrationBuilder.RenameTable(
                name: "Sites",
                newName: "VenueSites");

            migrationBuilder.RenameIndex(
                name: "IX_Sites_VenueId",
                table: "VenueSites",
                newName: "IX_VenueSites_VenueId");

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
    }
}
