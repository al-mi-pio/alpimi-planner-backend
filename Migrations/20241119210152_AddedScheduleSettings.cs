using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace alpimi_planner_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddedScheduleSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SchoolHour",
                table: "Schedule");

            migrationBuilder.CreateTable(
                name: "ScheduleSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SchoolHour = table.Column<int>(type: "int", nullable: false),
                    SchoolYearStart = table.Column<DateOnly>(type: "date", nullable: false),
                    SchoolYearEnd = table.Column<DateOnly>(type: "date", nullable: false),
                    ScheduleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScheduleSettings_Schedule_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "Schedule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleSettings_ScheduleId",
                table: "ScheduleSettings",
                column: "ScheduleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScheduleSettings");

            migrationBuilder.AddColumn<int>(
                name: "SchoolHour",
                table: "Schedule",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
