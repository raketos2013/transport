using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class changeLimit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Source",
                table: "TaskStep",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExecutionCount",
                table: "Task",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExecutionCount",
                table: "Task");

            migrationBuilder.AlterColumn<string>(
                name: "Source",
                table: "TaskStep",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
