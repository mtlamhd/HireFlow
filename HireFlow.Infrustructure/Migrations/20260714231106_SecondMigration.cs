using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HireFlow.Infrustructure.Migrations
{
    /// <inheritdoc />
    public partial class SecondMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Attachments_FilePath",
                table: "Attachments");

            migrationBuilder.AlterColumn<string>(
                name: "FilePath",
                table: "Attachments",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_FilePath",
                table: "Attachments",
                column: "FilePath",
                unique: true,
                filter: "[FilePath] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Attachments_FilePath",
                table: "Attachments");

            migrationBuilder.AlterColumn<string>(
                name: "FilePath",
                table: "Attachments",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_FilePath",
                table: "Attachments",
                column: "FilePath",
                unique: true);
        }
    }
}
