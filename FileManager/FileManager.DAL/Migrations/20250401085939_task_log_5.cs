using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileManager.DAL.Migrations
{
    /// <inheritdoc />
    public partial class task_log_5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskLog",
                table: "TaskLog");

            

            migrationBuilder.DropColumn(
                name: "OperationId",
                table: "TaskLog");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskLog",
                table: "TaskLog",
                columns: new[] { "DateTimeLog", "TaskId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskLog",
                table: "TaskLog");

            

            migrationBuilder.AddColumn<int>(
                name: "OperationId",
                table: "TaskLog",
                type: "integer",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskLog",
                table: "TaskLog",
                columns: new[] { "DateTimeLog", "TaskId", "StepId" });
        }
    }
}
