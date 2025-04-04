using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileManager.DAL.Migrations
{
    /// <inheritdoc />
    public partial class task_log_6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OperationId",
                table: "TaskLog",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StepId",
                table: "TaskLog",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OperationId",
                table: "TaskLog");

            migrationBuilder.DropColumn(
                name: "StepId",
                table: "TaskLog");
        }
    }
}
