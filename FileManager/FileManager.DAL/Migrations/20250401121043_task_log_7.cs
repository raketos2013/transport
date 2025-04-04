using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileManager.DAL.Migrations
{
    /// <inheritdoc />
    public partial class task_log_7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskLog",
                table: "TaskLog");

            migrationBuilder.DropColumn(
                name: "Test",
                table: "TaskLog");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskLog",
                table: "TaskLog",
                columns: new[] { "DateTimeLog", "TaskId", "StepId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskLog",
                table: "TaskLog");

            migrationBuilder.AddColumn<int>(
                name: "Test",
                table: "TaskLog",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskLog",
                table: "TaskLog",
                columns: new[] { "DateTimeLog", "TaskId" });
        }
    }
}
