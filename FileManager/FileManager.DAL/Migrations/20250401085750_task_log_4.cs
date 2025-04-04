using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileManager.DAL.Migrations
{
    /// <inheritdoc />
    public partial class task_log_4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Test",
                table: "TaskLog",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Test",
                table: "TaskLog");
        }
    }
}
