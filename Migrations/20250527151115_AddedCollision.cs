using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace alpimi_planner_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddedCollision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "CollisionType",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Collision",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CollidingObject1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CollidingObject2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ignored = table.Column<bool>(type: "bit", nullable: false),
                    CollisionTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Collision", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Collision_CollisionType_CollisionTypeId",
                        column: x => x.CollisionTypeId,
                        principalTable: "CollisionType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Collision_CollisionTypeId",
                table: "Collision",
                column: "CollisionTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Collision");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "CollisionType");
        }
    }
}
