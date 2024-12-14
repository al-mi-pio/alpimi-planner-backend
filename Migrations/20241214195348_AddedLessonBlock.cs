using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace alpimi_planner_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddedLessonBlock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LessonBlock",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LessonDate = table.Column<DateOnly>(type: "date", nullable: false),
                    LessonStart = table.Column<int>(type: "int", nullable: false),
                    LessonEnd = table.Column<int>(type: "int", nullable: false),
                    LessonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClassroomId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TeacherId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ClusterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonBlock", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LessonBlock_Classroom_ClassroomId",
                        column: x => x.ClassroomId,
                        principalTable: "Classroom",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LessonBlock_Lesson_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lesson",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LessonBlock_Teacher_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teacher",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_LessonBlock_ClassroomId",
                table: "LessonBlock",
                column: "ClassroomId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonBlock_LessonId",
                table: "LessonBlock",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonBlock_TeacherId",
                table: "LessonBlock",
                column: "TeacherId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LessonBlock");
        }
    }
}
