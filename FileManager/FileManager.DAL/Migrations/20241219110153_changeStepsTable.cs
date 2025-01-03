using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileManager.DAL.Migrations
{
    /// <inheritdoc />
    public partial class changeStepsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OperationCopy_Task_TaskEntityTaskId",
                table: "OperationCopy");

            migrationBuilder.DropForeignKey(
                name: "FK_OperationDelete_Task_TaskEntityTaskId",
                table: "OperationDelete");

            migrationBuilder.DropForeignKey(
                name: "FK_OperationExist_Task_TaskEntityTaskId",
                table: "OperationExist");

            migrationBuilder.DropForeignKey(
                name: "FK_OperationMove_Task_TaskEntityTaskId",
                table: "OperationMove");

            migrationBuilder.DropForeignKey(
                name: "FK_OperationRead_Task_TaskEntityTaskId",
                table: "OperationRead");

            migrationBuilder.DropForeignKey(
                name: "FK_OperationRename_Task_TaskEntityTaskId",
                table: "OperationRename");

            migrationBuilder.DropIndex(
                name: "IX_OperationRename_StepId",
                table: "OperationRename");

            migrationBuilder.DropIndex(
                name: "IX_OperationRename_TaskEntityTaskId",
                table: "OperationRename");

            migrationBuilder.DropIndex(
                name: "IX_OperationRead_StepId",
                table: "OperationRead");

            migrationBuilder.DropIndex(
                name: "IX_OperationRead_TaskEntityTaskId",
                table: "OperationRead");

            migrationBuilder.DropIndex(
                name: "IX_OperationMove_StepId",
                table: "OperationMove");

            migrationBuilder.DropIndex(
                name: "IX_OperationMove_TaskEntityTaskId",
                table: "OperationMove");

            migrationBuilder.DropIndex(
                name: "IX_OperationExist_StepId",
                table: "OperationExist");

            migrationBuilder.DropIndex(
                name: "IX_OperationExist_TaskEntityTaskId",
                table: "OperationExist");

            migrationBuilder.DropIndex(
                name: "IX_OperationDelete_StepId",
                table: "OperationDelete");

            migrationBuilder.DropIndex(
                name: "IX_OperationDelete_TaskEntityTaskId",
                table: "OperationDelete");

            migrationBuilder.DropIndex(
                name: "IX_OperationCopy_StepId",
                table: "OperationCopy");

            migrationBuilder.DropIndex(
                name: "IX_OperationCopy_TaskEntityTaskId",
                table: "OperationCopy");

            migrationBuilder.DropColumn(
                name: "StepNumber",
                table: "OperationRename");

            migrationBuilder.DropColumn(
                name: "TaskEntityTaskId",
                table: "OperationRename");

            migrationBuilder.DropColumn(
                name: "StepNumber",
                table: "OperationRead");

            migrationBuilder.DropColumn(
                name: "TaskEntityTaskId",
                table: "OperationRead");

            migrationBuilder.DropColumn(
                name: "StepNumber",
                table: "OperationMove");

            migrationBuilder.DropColumn(
                name: "TaskEntityTaskId",
                table: "OperationMove");

            migrationBuilder.DropColumn(
                name: "StepNumber",
                table: "OperationExist");

            migrationBuilder.DropColumn(
                name: "TaskEntityTaskId",
                table: "OperationExist");

            migrationBuilder.DropColumn(
                name: "StepNumber",
                table: "OperationDelete");

            migrationBuilder.DropColumn(
                name: "TaskEntityTaskId",
                table: "OperationDelete");

            migrationBuilder.DropColumn(
                name: "StepNumber",
                table: "OperationCopy");

            migrationBuilder.DropColumn(
                name: "TaskEntityTaskId",
                table: "OperationCopy");

            migrationBuilder.AddColumn<int>(
                name: "OperationId",
                table: "TaskStep",
                type: "integer",
                nullable: false,
                defaultValue: 0);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "OperationId",
                table: "TaskStep");

            migrationBuilder.AddColumn<int>(
                name: "StepNumber",
                table: "OperationRename",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TaskEntityTaskId",
                table: "OperationRename",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StepNumber",
                table: "OperationRead",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TaskEntityTaskId",
                table: "OperationRead",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StepNumber",
                table: "OperationMove",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TaskEntityTaskId",
                table: "OperationMove",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StepNumber",
                table: "OperationExist",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TaskEntityTaskId",
                table: "OperationExist",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StepNumber",
                table: "OperationDelete",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TaskEntityTaskId",
                table: "OperationDelete",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StepNumber",
                table: "OperationCopy",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TaskEntityTaskId",
                table: "OperationCopy",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OperationRename_StepId",
                table: "OperationRename",
                column: "StepId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationRename_TaskEntityTaskId",
                table: "OperationRename",
                column: "TaskEntityTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationRead_StepId",
                table: "OperationRead",
                column: "StepId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationRead_TaskEntityTaskId",
                table: "OperationRead",
                column: "TaskEntityTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationMove_StepId",
                table: "OperationMove",
                column: "StepId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationMove_TaskEntityTaskId",
                table: "OperationMove",
                column: "TaskEntityTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationExist_StepId",
                table: "OperationExist",
                column: "StepId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationExist_TaskEntityTaskId",
                table: "OperationExist",
                column: "TaskEntityTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationDelete_StepId",
                table: "OperationDelete",
                column: "StepId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationDelete_TaskEntityTaskId",
                table: "OperationDelete",
                column: "TaskEntityTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationCopy_StepId",
                table: "OperationCopy",
                column: "StepId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationCopy_TaskEntityTaskId",
                table: "OperationCopy",
                column: "TaskEntityTaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_OperationCopy_Task_TaskEntityTaskId",
                table: "OperationCopy",
                column: "TaskEntityTaskId",
                principalTable: "Task",
                principalColumn: "TaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_OperationDelete_Task_TaskEntityTaskId",
                table: "OperationDelete",
                column: "TaskEntityTaskId",
                principalTable: "Task",
                principalColumn: "TaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_OperationExist_Task_TaskEntityTaskId",
                table: "OperationExist",
                column: "TaskEntityTaskId",
                principalTable: "Task",
                principalColumn: "TaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_OperationMove_Task_TaskEntityTaskId",
                table: "OperationMove",
                column: "TaskEntityTaskId",
                principalTable: "Task",
                principalColumn: "TaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_OperationRead_Task_TaskEntityTaskId",
                table: "OperationRead",
                column: "TaskEntityTaskId",
                principalTable: "Task",
                principalColumn: "TaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_OperationRename_Task_TaskEntityTaskId",
                table: "OperationRename",
                column: "TaskEntityTaskId",
                principalTable: "Task",
                principalColumn: "TaskId");
        }
    }
}
