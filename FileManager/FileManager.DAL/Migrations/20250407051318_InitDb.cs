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
            migrationBuilder.CreateSequence(
                name: "TaskOperationSequence");

            migrationBuilder.CreateTable(
                name: "AddresseeGroup",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddresseeGroup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClientLog",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Values = table.Column<string>(type: "jsonb", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaskGroup",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskGroup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaskLog",
                columns: table => new
                {
                    DateTimeLog = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    TaskId = table.Column<string>(type: "text", nullable: false),
                    StepId = table.Column<int>(type: "integer", nullable: true),
                    OperationId = table.Column<int>(type: "integer", nullable: true),
                    ResultOperation = table.Column<int>(type: "integer", nullable: true),
                    FileName = table.Column<string>(type: "text", nullable: true),
                    ResultText = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskLog", x => new { x.DateTimeLog, x.TaskId });
                });

            migrationBuilder.CreateTable(
                name: "UserLog",
                columns: table => new
                {
                    DateTimeLog = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UserName = table.Column<string>(type: "text", nullable: false),
                    Action = table.Column<string>(type: "text", nullable: false),
                    Data = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLog", x => new { x.DateTimeLog, x.UserName });
                });

            migrationBuilder.CreateTable(
                name: "Addressee",
                columns: table => new
                {
                    PersonalNumber = table.Column<string>(type: "text", nullable: false),
                    AddresseeGroupId = table.Column<int>(type: "integer", nullable: false),
                    EMail = table.Column<string>(type: "text", nullable: false),
                    Fio = table.Column<string>(type: "text", nullable: false),
                    StructuralUnit = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addressee", x => new { x.PersonalNumber, x.AddresseeGroupId });
                    table.ForeignKey(
                        name: "FK_Addressee_AddresseeGroup_AddresseeGroupId",
                        column: x => x.AddresseeGroupId,
                        principalTable: "AddresseeGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Task",
                columns: table => new
                {
                    TaskId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    TimeBegin = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    TimeEnd = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    DayActive = table.Column<int>(type: "integer", nullable: false),
                    AddresseeGroupId = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    TaskGroupId = table.Column<int>(type: "integer", nullable: false),
                    ExecutionLimit = table.Column<int>(type: "integer", nullable: false),
                    ExecutionLeft = table.Column<int>(type: "integer", nullable: false),
                    IsProgress = table.Column<bool>(type: "boolean", nullable: false),
                    TaskGroupEntityId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Task", x => x.TaskId);
                    table.ForeignKey(
                        name: "FK_Task_TaskGroup_TaskGroupEntityId",
                        column: x => x.TaskGroupEntityId,
                        principalTable: "TaskGroup",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TaskStatuse",
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
                    table.PrimaryKey("PK_TaskStatuse", x => x.TaskId);
                    table.ForeignKey(
                        name: "FK_TaskStatuse_Task_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Task",
                        principalColumn: "TaskId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskStep",
                columns: table => new
                {
                    StepId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TaskId = table.Column<string>(type: "text", nullable: false),
                    OperationId = table.Column<int>(type: "integer", nullable: false),
                    OperationName = table.Column<int>(type: "integer", nullable: false),
                    StepNumber = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    FileMask = table.Column<string>(type: "text", nullable: false),
                    Source = table.Column<string>(type: "text", nullable: false),
                    Destination = table.Column<string>(type: "text", nullable: false),
                    IsBreak = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskStep", x => x.StepId);
                    table.ForeignKey(
                        name: "FK_TaskStep_Task_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Task",
                        principalColumn: "TaskId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OperationCopy",
                columns: table => new
                {
                    OperationId = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"TaskOperationSequence\"')"),
                    StepId = table.Column<int>(type: "integer", nullable: false),
                    InformSuccess = table.Column<bool>(type: "boolean", nullable: false),
                    AddresseeGroupId = table.Column<int>(type: "integer", nullable: true),
                    AdditionalText = table.Column<string>(type: "text", nullable: true),
                    FileInSource = table.Column<int>(type: "integer", nullable: false),
                    FileInDestination = table.Column<int>(type: "integer", nullable: false),
                    FileInLog = table.Column<bool>(type: "boolean", nullable: false),
                    Sort = table.Column<int>(type: "integer", nullable: false),
                    FilesForProcessing = table.Column<int>(type: "integer", nullable: false),
                    FileAttribute = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationCopy", x => x.OperationId);
                    table.ForeignKey(
                        name: "FK_OperationCopy_TaskStep_StepId",
                        column: x => x.StepId,
                        principalTable: "TaskStep",
                        principalColumn: "StepId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OperationDelete",
                columns: table => new
                {
                    OperationId = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"TaskOperationSequence\"')"),
                    StepId = table.Column<int>(type: "integer", nullable: false),
                    InformSuccess = table.Column<bool>(type: "boolean", nullable: false),
                    AddresseeGroupId = table.Column<int>(type: "integer", nullable: true),
                    AdditionalText = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationDelete", x => x.OperationId);
                    table.ForeignKey(
                        name: "FK_OperationDelete_TaskStep_StepId",
                        column: x => x.StepId,
                        principalTable: "TaskStep",
                        principalColumn: "StepId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OperationExist",
                columns: table => new
                {
                    OperationId = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"TaskOperationSequence\"')"),
                    StepId = table.Column<int>(type: "integer", nullable: false),
                    InformSuccess = table.Column<bool>(type: "boolean", nullable: false),
                    AddresseeGroupId = table.Column<int>(type: "integer", nullable: true),
                    AdditionalText = table.Column<string>(type: "text", nullable: true),
                    ExpectedResult = table.Column<int>(type: "integer", nullable: false),
                    BreakTaskAfterError = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationExist", x => x.OperationId);
                    table.ForeignKey(
                        name: "FK_OperationExist_TaskStep_StepId",
                        column: x => x.StepId,
                        principalTable: "TaskStep",
                        principalColumn: "StepId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OperationMove",
                columns: table => new
                {
                    OperationId = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"TaskOperationSequence\"')"),
                    StepId = table.Column<int>(type: "integer", nullable: false),
                    InformSuccess = table.Column<bool>(type: "boolean", nullable: false),
                    AddresseeGroupId = table.Column<int>(type: "integer", nullable: true),
                    AdditionalText = table.Column<string>(type: "text", nullable: true),
                    FileInDestination = table.Column<int>(type: "integer", nullable: false),
                    FileInLog = table.Column<bool>(type: "boolean", nullable: false),
                    Sort = table.Column<int>(type: "integer", nullable: false),
                    FilesForProcessing = table.Column<int>(type: "integer", nullable: false),
                    FileAttribute = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationMove", x => x.OperationId);
                    table.ForeignKey(
                        name: "FK_OperationMove_TaskStep_StepId",
                        column: x => x.StepId,
                        principalTable: "TaskStep",
                        principalColumn: "StepId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OperationRead",
                columns: table => new
                {
                    OperationId = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"TaskOperationSequence\"')"),
                    StepId = table.Column<int>(type: "integer", nullable: false),
                    InformSuccess = table.Column<bool>(type: "boolean", nullable: false),
                    AddresseeGroupId = table.Column<int>(type: "integer", nullable: true),
                    AdditionalText = table.Column<string>(type: "text", nullable: true),
                    FileInSource = table.Column<int>(type: "integer", nullable: false),
                    Encode = table.Column<int>(type: "integer", nullable: false),
                    SearchRegex = table.Column<bool>(type: "boolean", nullable: false),
                    FindString = table.Column<string>(type: "text", nullable: false),
                    ExpectedResult = table.Column<int>(type: "integer", nullable: false),
                    BreakTaskAfterError = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationRead", x => x.OperationId);
                    table.ForeignKey(
                        name: "FK_OperationRead_TaskStep_StepId",
                        column: x => x.StepId,
                        principalTable: "TaskStep",
                        principalColumn: "StepId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OperationRename",
                columns: table => new
                {
                    OperationId = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"TaskOperationSequence\"')"),
                    StepId = table.Column<int>(type: "integer", nullable: false),
                    InformSuccess = table.Column<bool>(type: "boolean", nullable: false),
                    AddresseeGroupId = table.Column<int>(type: "integer", nullable: true),
                    AdditionalText = table.Column<string>(type: "text", nullable: true),
                    Pattern = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationRename", x => x.OperationId);
                    table.ForeignKey(
                        name: "FK_OperationRename_TaskStep_StepId",
                        column: x => x.StepId,
                        principalTable: "TaskStep",
                        principalColumn: "StepId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Addressee_AddresseeGroupId",
                table: "Addressee",
                column: "AddresseeGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationCopy_StepId",
                table: "OperationCopy",
                column: "StepId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationDelete_StepId",
                table: "OperationDelete",
                column: "StepId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationExist_StepId",
                table: "OperationExist",
                column: "StepId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationMove_StepId",
                table: "OperationMove",
                column: "StepId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationRead_StepId",
                table: "OperationRead",
                column: "StepId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationRename_StepId",
                table: "OperationRename",
                column: "StepId");

            migrationBuilder.CreateIndex(
                name: "IX_Task_TaskGroupEntityId",
                table: "Task",
                column: "TaskGroupEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskStep_TaskId",
                table: "TaskStep",
                column: "TaskId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Addressee");

            migrationBuilder.DropTable(
                name: "ClientLog");

            migrationBuilder.DropTable(
                name: "OperationCopy");

            migrationBuilder.DropTable(
                name: "OperationDelete");

            migrationBuilder.DropTable(
                name: "OperationExist");

            migrationBuilder.DropTable(
                name: "OperationMove");

            migrationBuilder.DropTable(
                name: "OperationRead");

            migrationBuilder.DropTable(
                name: "OperationRename");

            migrationBuilder.DropTable(
                name: "TaskLog");

            migrationBuilder.DropTable(
                name: "TaskStatuse");

            migrationBuilder.DropTable(
                name: "UserLog");

            migrationBuilder.DropTable(
                name: "AddresseeGroup");

            migrationBuilder.DropTable(
                name: "TaskStep");

            migrationBuilder.DropTable(
                name: "Task");

            migrationBuilder.DropTable(
                name: "TaskGroup");

            migrationBuilder.DropSequence(
                name: "TaskOperationSequence");
        }
    }
}
