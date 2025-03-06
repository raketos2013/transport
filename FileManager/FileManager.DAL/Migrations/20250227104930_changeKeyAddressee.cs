using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileManager.DAL.Migrations
{
    /// <inheritdoc />
    public partial class changeKeyAddressee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Addressee",
                table: "Addressee");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Addressee",
                table: "Addressee",
                columns: new[] { "PersonalNumber", "AddresseeGroupId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Addressee",
                table: "Addressee");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Addressee",
                table: "Addressee",
                column: "PersonalNumber");
        }
    }
}
