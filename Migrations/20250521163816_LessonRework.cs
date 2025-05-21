using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace alpimi_planner_backend.Migrations
{
    /// <inheritdoc />
    public partial class LessonRework : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lesson_Subgroup_SubgroupId",
                table: "Lesson");

            migrationBuilder.DropForeignKey(
                name: "FK_LessonBlock_Teacher_TeacherId",
                table: "LessonBlock");

            migrationBuilder.DropIndex(
                name: "IX_LessonBlock_TeacherId",
                table: "LessonBlock");

            migrationBuilder.DropColumn(
                name: "TeacherId",
                table: "LessonBlock");

            migrationBuilder.RenameColumn(
                name: "SubgroupId",
                table: "Lesson",
                newName: "TeacherId");

            migrationBuilder.RenameIndex(
                name: "IX_Lesson_SubgroupId",
                table: "Lesson",
                newName: "IX_Lesson_TeacherId");

            migrationBuilder.CreateTable(
                name: "LessonSubgroup",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LessonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubgroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonSubgroup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LessonSubgroup_Lesson_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lesson",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LessonSubgroup_LessonId",
                table: "LessonSubgroup",
                column: "LessonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lesson_Teacher_TeacherId",
                table: "Lesson",
                column: "TeacherId",
                principalTable: "Teacher",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lesson_Teacher_TeacherId",
                table: "Lesson");

            migrationBuilder.DropTable(
                name: "LessonSubgroup");

            migrationBuilder.RenameColumn(
                name: "TeacherId",
                table: "Lesson",
                newName: "SubgroupId");

            migrationBuilder.RenameIndex(
                name: "IX_Lesson_TeacherId",
                table: "Lesson",
                newName: "IX_Lesson_SubgroupId");

            migrationBuilder.AddColumn<Guid>(
                name: "TeacherId",
                table: "LessonBlock",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LessonBlock_TeacherId",
                table: "LessonBlock",
                column: "TeacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lesson_Subgroup_SubgroupId",
                table: "Lesson",
                column: "SubgroupId",
                principalTable: "Subgroup",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LessonBlock_Teacher_TeacherId",
                table: "LessonBlock",
                column: "TeacherId",
                principalTable: "Teacher",
                principalColumn: "Id");
        }
    }
}
