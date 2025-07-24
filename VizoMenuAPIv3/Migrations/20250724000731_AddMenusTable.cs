using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VizoMenuAPIv3.Migrations
{
    /// <inheritdoc />
    public partial class AddMenusTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Menus",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MenuName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    DisplayText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdditionalText1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdditionalText2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    EnteredBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EnteredUTC = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastChangedUTC = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastChngedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ButtonImageId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Menus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Menus_Sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Menus_SiteId",
                table: "Menus",
                column: "SiteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Menus");
        }
    }
}
