using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    
    public partial class AddEmailAndPhoneNumberToUser : Migration
    {
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                schema: "dbo",
                table: "LibraryUsers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                schema: "dbo",
                table: "LibraryUsers",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LibraryUsers_Email",
                schema: "dbo",
                table: "LibraryUsers",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");
        }

        
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LibraryUsers_Email",
                schema: "dbo",
                table: "LibraryUsers");

            migrationBuilder.DropColumn(
                name: "Email",
                schema: "dbo",
                table: "LibraryUsers");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                schema: "dbo",
                table: "LibraryUsers");
        }
    }
}
