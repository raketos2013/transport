using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileManager.DAL.Migrations
{
    /// <inheritdoc />
    public partial class addActiveAddressee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addressee_AddresseeGroup_MailGroupId",
                table: "Addressee");

            migrationBuilder.RenameColumn(
                name: "MailGroupId",
                table: "Addressee",
                newName: "AddresseeGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_Addressee_MailGroupId",
                table: "Addressee",
                newName: "IX_Addressee_AddresseeGroupId");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Addressee",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Addressee_AddresseeGroup_AddresseeGroupId",
                table: "Addressee",
                column: "AddresseeGroupId",
                principalTable: "AddresseeGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addressee_AddresseeGroup_AddresseeGroupId",
                table: "Addressee");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Addressee");

            migrationBuilder.RenameColumn(
                name: "AddresseeGroupId",
                table: "Addressee",
                newName: "MailGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_Addressee_AddresseeGroupId",
                table: "Addressee",
                newName: "IX_Addressee_MailGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Addressee_AddresseeGroup_MailGroupId",
                table: "Addressee",
                column: "MailGroupId",
                principalTable: "AddresseeGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
