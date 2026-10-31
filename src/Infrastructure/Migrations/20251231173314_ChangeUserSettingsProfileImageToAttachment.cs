using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    
    public partial class ChangeUserSettingsProfileImageToAttachment : Migration
    {
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfileImagePath",
                schema: "dbo",
                table: "UserSettings");

            migrationBuilder.AddColumn<long>(
                name: "ProfileImageAttachmentId",
                schema: "dbo",
                table: "UserSettings",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSettings_ProfileImageAttachmentId",
                schema: "dbo",
                table: "UserSettings",
                column: "ProfileImageAttachmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserSettings_Attachment_ProfileImageAttachmentId",
                schema: "dbo",
                table: "UserSettings",
                column: "ProfileImageAttachmentId",
                principalSchema: "dbo",
                principalTable: "Attachment",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserSettings_Attachment_ProfileImageAttachmentId",
                schema: "dbo",
                table: "UserSettings");

            migrationBuilder.DropIndex(
                name: "IX_UserSettings_ProfileImageAttachmentId",
                schema: "dbo",
                table: "UserSettings");

            migrationBuilder.DropColumn(
                name: "ProfileImageAttachmentId",
                schema: "dbo",
                table: "UserSettings");

            migrationBuilder.AddColumn<string>(
                name: "ProfileImagePath",
                schema: "dbo",
                table: "UserSettings",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }
    }
}
