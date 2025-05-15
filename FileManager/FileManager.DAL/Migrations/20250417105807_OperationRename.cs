using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileManager.DAL.Migrations
{
    /// <inheritdoc />
    public partial class OperationRename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Pattern",
                table: "OperationRename",
                newName: "OldPattern");

            migrationBuilder.AlterColumn<string>(
                name: "Source",
                table: "TaskStep",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "NewPattern",
                table: "OperationRename",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NewPattern",
                table: "OperationRename");

            migrationBuilder.RenameColumn(
                name: "OldPattern",
                table: "OperationRename",
                newName: "Pattern");

            migrationBuilder.AlterColumn<string>(
                name: "Source",
                table: "TaskStep",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
