using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    
    public partial class AddIsMainToAttachment : Migration
    {
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsMain",
                schema: "dbo",
                table: "Attachment",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsMain",
                schema: "dbo",
                table: "Attachment");
        }
    }
}
