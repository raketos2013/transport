using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileManager.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTaskLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OperationName",
                table: "TaskLog",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StepNumber",
                table: "TaskLog",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OperationName",
                table: "TaskLog");

            migrationBuilder.DropColumn(
                name: "StepNumber",
                table: "TaskLog");
        }
    }
}
