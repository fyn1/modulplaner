using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FhModulplaner.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDbContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OpenTimetables",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TimetableId = table.Column<Guid>(type: "TEXT", nullable: false),
                    OpendTimetableId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpenTimetables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OpenTimetables_Timetables_OpendTimetableId",
                        column: x => x.OpendTimetableId,
                        principalTable: "Timetables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OpenTimetables_Timetables_TimetableId",
                        column: x => x.TimetableId,
                        principalTable: "Timetables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OpenTimetables_OpendTimetableId",
                table: "OpenTimetables",
                column: "OpendTimetableId");

            migrationBuilder.CreateIndex(
                name: "IX_OpenTimetables_TimetableId",
                table: "OpenTimetables",
                column: "TimetableId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OpenTimetables");
        }
    }
}
