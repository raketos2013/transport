using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileManager.DAL.Migrations
{
    /// <inheritdoc />
    public partial class add_clrbuf : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OperationClrbuf",
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
                    table.PrimaryKey("PK_OperationClrbuf", x => x.OperationId);
                    table.ForeignKey(
                        name: "FK_OperationClrbuf_TaskStep_StepId",
                        column: x => x.StepId,
                        principalTable: "TaskStep",
                        principalColumn: "StepId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OperationClrbuf_StepId",
                table: "OperationClrbuf",
                column: "StepId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OperationClrbuf");
        }
    }
}
