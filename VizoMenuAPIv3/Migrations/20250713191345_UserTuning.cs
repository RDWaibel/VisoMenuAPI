using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VizoMenuAPIv3.Migrations
{
    /// <inheritdoc />
    public partial class UserTuning : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DisabledById",
                table: "Users",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DisabledUTC",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsEnabled",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisabledById",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DisabledUTC",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsEnabled",
                table: "Users");
        }
    }
}
