using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FileManager.DAL.Migrations
{
    /// <inheritdoc />
    public partial class InitDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClientLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Values = table.Column<string>(type: "jsonb", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MailGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaskGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransportTaskLogs",
                columns: table => new
                {
                    DateTimeLog = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    TaskId = table.Column<string>(type: "text", nullable: false),
                    OperationId = table.Column<string>(type: "text", nullable: false),
                    ResultOperation = table.Column<int>(type: "integer", nullable: false),
                    FileName = table.Column<string>(type: "text", nullable: false),
                    ResultText = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransportTaskLogs", x => new { x.DateTimeLog, x.TaskId, x.OperationId });
                });

            migrationBuilder.CreateTable(
                name: "UserLogs",
                columns: table => new
                {
                    DateTimeLog = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UserName = table.Column<string>(type: "text", nullable: false),
                    Action = table.Column<string>(type: "text", nullable: false),
                    Data = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogs", x => new { x.DateTimeLog, x.UserName });
                });

            migrationBuilder.CreateTable(
                name: "MailLists",
                columns: table => new
                {
                    MailGroupsId = table.Column<int>(type: "integer", nullable: false),
                    EMail = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailLists", x => new { x.MailGroupsId, x.EMail });
                    table.ForeignKey(
                        name: "FK_MailLists_MailGroups_MailGroupsId",
                        column: x => x.MailGroupsId,
                        principalTable: "MailGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    TaskId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    TimeBegin = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    TimeEnd = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    DayActive = table.Column<int>(type: "integer", nullable: false),
                    Group = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    SourceCatalog = table.Column<string>(type: "text", nullable: false),
                    FileMask = table.Column<string>(type: "text", nullable: false),
                    Delay = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    ArchiveCatalog = table.Column<string>(type: "text", nullable: false),
                    BadArchiveCatalog = table.Column<string>(type: "text", nullable: false),
                    IsDeleteSource = table.Column<bool>(type: "boolean", nullable: false),
                    MaxAmountFiles = table.Column<int>(type: "integer", nullable: false),
                    DublNameJr = table.Column<bool>(type: "boolean", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    SplitFiles = table.Column<bool>(type: "boolean", nullable: false),
                    IsRegex = table.Column<bool>(type: "boolean", nullable: false),
                    ValueParameterOfSplit = table.Column<string>(type: "text", nullable: true),
                    MoveToTmp = table.Column<bool>(type: "boolean", nullable: false),
                    TmpCatalog = table.Column<string>(type: "text", nullable: true),
                    DelayBeforeExecuting = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    SubMask = table.Column<string>(type: "text", nullable: true),
                    TaskGroup = table.Column<int>(type: "integer", nullable: false),
                    TaskGroupEntityId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.TaskId);
                    table.ForeignKey(
                        name: "FK_Tasks_TaskGroups_TaskGroupEntityId",
                        column: x => x.TaskGroupEntityId,
                        principalTable: "TaskGroups",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TaskOperations",
                columns: table => new
                {
                    OperationId = table.Column<string>(type: "text", nullable: false),
                    TaskId = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    DestinationDirectory = table.Column<string>(type: "text", nullable: false),
                    IsRename = table.Column<bool>(type: "boolean", nullable: false),
                    TemplateFileName = table.Column<string>(type: "text", nullable: true),
                    NewTemplateFileName = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Group = table.Column<int>(type: "integer", nullable: true),
                    DublDest = table.Column<int>(type: "integer", nullable: false),
                    DublNameJr = table.Column<bool>(type: "boolean", nullable: false),
                    AdditionalText = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskOperations", x => new { x.OperationId, x.TaskId });
                    table.ForeignKey(
                        name: "FK_TaskOperations_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "TaskId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskStatuses",
                columns: table => new
                {
                    TaskId = table.Column<string>(type: "text", nullable: false),
                    IsProgress = table.Column<bool>(type: "boolean", nullable: false),
                    IsError = table.Column<bool>(type: "boolean", nullable: false),
                    CountExecute = table.Column<int>(type: "integer", nullable: false),
                    CountProcessedFiles = table.Column<int>(type: "integer", nullable: false),
                    DateLastExecute = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CountLeftFiles = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskStatuses", x => x.TaskId);
                    table.ForeignKey(
                        name: "FK_TaskStatuses_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "TaskId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskOperations_TaskId",
                table: "TaskOperations",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_TaskGroupEntityId",
                table: "Tasks",
                column: "TaskGroupEntityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientLogs");

            migrationBuilder.DropTable(
                name: "MailLists");

            migrationBuilder.DropTable(
                name: "TaskOperations");

            migrationBuilder.DropTable(
                name: "TaskStatuses");

            migrationBuilder.DropTable(
                name: "TransportTaskLogs");

            migrationBuilder.DropTable(
                name: "UserLogs");

            migrationBuilder.DropTable(
                name: "MailGroups");

            migrationBuilder.DropTable(
                name: "Tasks");

            migrationBuilder.DropTable(
                name: "TaskGroups");
        }
    }
}
