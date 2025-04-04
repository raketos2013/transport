using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileManager.DAL.Migrations
{
    /// <inheritdoc />
    public partial class chenge_PK_taskLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskLog",
                table: "TaskLog");

            migrationBuilder.AlterColumn<int>(
                name: "StepId",
                table: "TaskLog",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

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

            migrationBuilder.AlterColumn<int>(
                name: "StepId",
                table: "TaskLog",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskLog",
                table: "TaskLog",
                columns: new[] { "DateTimeLog", "TaskId", "StepId" });
        }
    }
}
