using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileManager.DAL.Migrations
{
    /// <inheritdoc />
    public partial class task_log3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TaskStep_OperationId",
                table: "TaskStep");

            migrationBuilder.DropIndex(
                name: "IX_OperationRename_StepId",
                table: "OperationRename");

            migrationBuilder.DropIndex(
                name: "IX_OperationRead_StepId",
                table: "OperationRead");

            migrationBuilder.DropIndex(
                name: "IX_OperationMove_StepId",
                table: "OperationMove");

            migrationBuilder.DropIndex(
                name: "IX_OperationExist_StepId",
                table: "OperationExist");

            migrationBuilder.DropIndex(
                name: "IX_OperationDelete_StepId",
                table: "OperationDelete");

            migrationBuilder.DropIndex(
                name: "IX_OperationCopy_StepId",
                table: "OperationCopy");

            migrationBuilder.CreateIndex(
                name: "IX_OperationRename_StepId",
                table: "OperationRename",
                column: "StepId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationRead_StepId",
                table: "OperationRead",
                column: "StepId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationMove_StepId",
                table: "OperationMove",
                column: "StepId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationExist_StepId",
                table: "OperationExist",
                column: "StepId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationDelete_StepId",
                table: "OperationDelete",
                column: "StepId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationCopy_StepId",
                table: "OperationCopy",
                column: "StepId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OperationRename_StepId",
                table: "OperationRename");

            migrationBuilder.DropIndex(
                name: "IX_OperationRead_StepId",
                table: "OperationRead");

            migrationBuilder.DropIndex(
                name: "IX_OperationMove_StepId",
                table: "OperationMove");

            migrationBuilder.DropIndex(
                name: "IX_OperationExist_StepId",
                table: "OperationExist");

            migrationBuilder.DropIndex(
                name: "IX_OperationDelete_StepId",
                table: "OperationDelete");

            migrationBuilder.DropIndex(
                name: "IX_OperationCopy_StepId",
                table: "OperationCopy");

            migrationBuilder.CreateIndex(
                name: "IX_TaskStep_OperationId",
                table: "TaskStep",
                column: "OperationId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationRename_StepId",
                table: "OperationRename",
                column: "StepId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OperationRead_StepId",
                table: "OperationRead",
                column: "StepId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OperationMove_StepId",
                table: "OperationMove",
                column: "StepId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OperationExist_StepId",
                table: "OperationExist",
                column: "StepId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OperationDelete_StepId",
                table: "OperationDelete",
                column: "StepId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OperationCopy_StepId",
                table: "OperationCopy",
                column: "StepId",
                unique: true);
        }
    }
}
