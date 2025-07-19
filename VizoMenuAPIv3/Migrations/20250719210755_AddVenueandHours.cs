using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VizoMenuAPIv3.Migrations
{
    /// <inheritdoc />
    public partial class AddVenueandHours : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Venues");

            migrationBuilder.AddColumn<Guid>(
                name: "DisabledById",
                table: "Venues",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DisabledUTC",
                table: "Venues",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EnteredById",
                table: "Venues",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EnteredUTC",
                table: "Venues",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Is24Hours",
                table: "Venues",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Venues",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VenueName",
                table: "Venues",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "VenueHours",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VenueId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DayOfWeek = table.Column<int>(type: "int", nullable: false),
                    OpenTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    CloseTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    IsClosed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VenueHours", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VenueHours_Venues_VenueId",
                        column: x => x.VenueId,
                        principalTable: "Venues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VenueHours_VenueId",
                table: "VenueHours",
                column: "VenueId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VenueHours");

            migrationBuilder.DropColumn(
                name: "DisabledById",
                table: "Venues");

            migrationBuilder.DropColumn(
                name: "DisabledUTC",
                table: "Venues");

            migrationBuilder.DropColumn(
                name: "EnteredById",
                table: "Venues");

            migrationBuilder.DropColumn(
                name: "EnteredUTC",
                table: "Venues");

            migrationBuilder.DropColumn(
                name: "Is24Hours",
                table: "Venues");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Venues");

            migrationBuilder.DropColumn(
                name: "VenueName",
                table: "Venues");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Venues",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
