using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace alpimi_planner_backend.Migrations
{
    /// <inheritdoc />
    public partial class JoinSubgroups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "JointSubgroupId",
                table: "Subgroup",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JointSubgroupId",
                table: "Subgroup");
        }
    }
}
