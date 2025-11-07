using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable
#pragma warning disable CA1861 // Prefer 'static readonly' fields over constant array arguments

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdAndPasswordHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_LibraryUsers",
                schema: "dbo",
                table: "LibraryUsers");

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "LibraryUsers",
                keyColumn: "UserName",
                keyValue: "admin");

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                schema: "dbo",
                table: "LibraryUsers",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AddColumn<long>(
                name: "Id",
                schema: "dbo",
                table: "LibraryUsers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LibraryUsers",
                schema: "dbo",
                table: "LibraryUsers",
                column: "Id");

            migrationBuilder.InsertData(
                schema: "dbo",
                table: "LibraryUsers",
                columns: new[] { "Id", "LastName", "Name", "Password", "UserName" },
                values: new object[] { 1L, "admin", "admin", "9843417E11AFA30B39EB2E974DC2DCB2E53D4B608E665E478A1686C065107225-71815962DF6849A69BE50976ECA3DA44", "admin" });

            migrationBuilder.CreateIndex(
                name: "IX_LibraryUsers_UserName",
                schema: "dbo",
                table: "LibraryUsers",
                column: "UserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_LibraryUsers",
                schema: "dbo",
                table: "LibraryUsers");

            migrationBuilder.DropIndex(
                name: "IX_LibraryUsers_UserName",
                schema: "dbo",
                table: "LibraryUsers");

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "LibraryUsers",
                keyColumn: "Id",
                keyColumnType: "bigint",
                keyValue: 1L);

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "dbo",
                table: "LibraryUsers");

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                schema: "dbo",
                table: "LibraryUsers",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AddPrimaryKey(
                name: "PK_LibraryUsers",
                schema: "dbo",
                table: "LibraryUsers",
                column: "UserName");

            migrationBuilder.InsertData(
                schema: "dbo",
                table: "LibraryUsers",
                columns: new[] { "UserName", "LastName", "Name", "Password" },
                values: new object[] { "admin", "admin", "admin", "1234" });
        }
    }
}
#pragma warning restore CA1861 // Prefer 'static readonly' fields over constant array arguments
