using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VizoMenuAPIv3.Migrations
{
    /// <inheritdoc />
    public partial class AddOrgAddressFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Organizations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Organizations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContactPhone",
                table: "Organizations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EnteredBy",
                table: "Organizations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "EnteredUTC",
                table: "Organizations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "Organizations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ZipCode",
                table: "Organizations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "ContactPhone",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "EnteredBy",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "EnteredUTC",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "ZipCode",
                table: "Organizations");
        }
    }
}
