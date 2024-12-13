using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace alpimi_planner_backend.Migrations
{
    /// <inheritdoc />
    public partial class LessonBlokPrep : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Finish",
                table: "LessonPeriod");

            migrationBuilder.AddColumn<string>(
                name: "SchoolDays",
                table: "ScheduleSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "CurrentHours",
                table: "Lesson",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Lesson_SubgroupId",
                table: "Lesson",
                column: "SubgroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lesson_Subgroup_SubgroupId",
                table: "Lesson",
                column: "SubgroupId",
                principalTable: "Subgroup",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lesson_Subgroup_SubgroupId",
                table: "Lesson");

            migrationBuilder.DropIndex(
                name: "IX_Lesson_SubgroupId",
                table: "Lesson");

            migrationBuilder.DropColumn(
                name: "SchoolDays",
                table: "ScheduleSettings");

            migrationBuilder.DropColumn(
                name: "CurrentHours",
                table: "Lesson");

            migrationBuilder.AddColumn<TimeOnly>(
                name: "Finish",
                table: "LessonPeriod",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));
        }
    }
}
