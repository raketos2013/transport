using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileManager.DAL.Migrations
{
    /// <inheritdoc />
    public partial class changeOperation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OperationCopy_AddresseeGroup_AddresseeGroupId",
                table: "OperationCopy");

            migrationBuilder.DropForeignKey(
                name: "FK_OperationDelete_AddresseeGroup_AddresseeGroupId",
                table: "OperationDelete");

            migrationBuilder.DropForeignKey(
                name: "FK_OperationExist_AddresseeGroup_AddresseeGroupId",
                table: "OperationExist");

            migrationBuilder.DropForeignKey(
                name: "FK_OperationMove_AddresseeGroup_AddresseeGroupId",
                table: "OperationMove");

            migrationBuilder.DropForeignKey(
                name: "FK_OperationRead_AddresseeGroup_AddresseeGroupId",
                table: "OperationRead");

            migrationBuilder.DropForeignKey(
                name: "FK_OperationRename_AddresseeGroup_AddresseeGroupId",
                table: "OperationRename");

            migrationBuilder.DropIndex(
                name: "IX_OperationRename_AddresseeGroupId",
                table: "OperationRename");

            migrationBuilder.DropIndex(
                name: "IX_OperationRead_AddresseeGroupId",
                table: "OperationRead");

            migrationBuilder.DropIndex(
                name: "IX_OperationMove_AddresseeGroupId",
                table: "OperationMove");

            migrationBuilder.DropIndex(
                name: "IX_OperationExist_AddresseeGroupId",
                table: "OperationExist");

            migrationBuilder.DropIndex(
                name: "IX_OperationDelete_AddresseeGroupId",
                table: "OperationDelete");

            migrationBuilder.DropIndex(
                name: "IX_OperationCopy_AddresseeGroupId",
                table: "OperationCopy");

            migrationBuilder.AlterColumn<int>(
                name: "AddresseeGroupId",
                table: "OperationRename",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "AddresseeGroupId",
                table: "OperationRead",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "AddresseeGroupId",
                table: "OperationMove",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "AddresseeGroupId",
                table: "OperationExist",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "AddresseeGroupId",
                table: "OperationDelete",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "AddresseeGroupId",
                table: "OperationCopy",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "AddresseeGroupId",
                table: "OperationRename",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AddresseeGroupId",
                table: "OperationRead",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AddresseeGroupId",
                table: "OperationMove",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AddresseeGroupId",
                table: "OperationExist",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AddresseeGroupId",
                table: "OperationDelete",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AddresseeGroupId",
                table: "OperationCopy",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OperationRename_AddresseeGroupId",
                table: "OperationRename",
                column: "AddresseeGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationRead_AddresseeGroupId",
                table: "OperationRead",
                column: "AddresseeGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationMove_AddresseeGroupId",
                table: "OperationMove",
                column: "AddresseeGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationExist_AddresseeGroupId",
                table: "OperationExist",
                column: "AddresseeGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationDelete_AddresseeGroupId",
                table: "OperationDelete",
                column: "AddresseeGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationCopy_AddresseeGroupId",
                table: "OperationCopy",
                column: "AddresseeGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_OperationCopy_AddresseeGroup_AddresseeGroupId",
                table: "OperationCopy",
                column: "AddresseeGroupId",
                principalTable: "AddresseeGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OperationDelete_AddresseeGroup_AddresseeGroupId",
                table: "OperationDelete",
                column: "AddresseeGroupId",
                principalTable: "AddresseeGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OperationExist_AddresseeGroup_AddresseeGroupId",
                table: "OperationExist",
                column: "AddresseeGroupId",
                principalTable: "AddresseeGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OperationMove_AddresseeGroup_AddresseeGroupId",
                table: "OperationMove",
                column: "AddresseeGroupId",
                principalTable: "AddresseeGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OperationRead_AddresseeGroup_AddresseeGroupId",
                table: "OperationRead",
                column: "AddresseeGroupId",
                principalTable: "AddresseeGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OperationRename_AddresseeGroup_AddresseeGroupId",
                table: "OperationRename",
                column: "AddresseeGroupId",
                principalTable: "AddresseeGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
