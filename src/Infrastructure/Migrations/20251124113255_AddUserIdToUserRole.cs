using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional
#pragma warning disable CA1861 // Prefer 'static readonly' fields over constant array arguments

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToUserRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "UserId",
                schema: "dbo",
                table: "UserRole",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.Sql(@"
                UPDATE ur
                SET UserId = u.Id
                FROM [dbo].[UserRole] ur
                INNER JOIN [dbo].[LibraryUsers] u ON u.UserName = ur.UserName;
            ");

            migrationBuilder.InsertData(
                schema: "dbo",
                table: "Role",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 3, "Librarian" },
                    { 4, "Publisher" },
                    { 5, "Author" }
                });

            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "UserRole",
                keyColumn: "Id",
                keyValue: 1,
                column: "UserId",
                value: 1L);

            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "UserRole",
                keyColumn: "Id",
                keyValue: 2,
                column: "UserId",
                value: 1L);

            migrationBuilder.InsertData(
                schema: "dbo",
                table: "UserRole",
                columns: new[] { "Id", "RoleId", "UserId", "UserName" },
                values: new object[,]
                {
                    { 3, 3, 1L, "admin" },
                    { 4, 4, 1L, "admin" },
                    { 5, 5, 1L, "admin" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_UserId_RoleId",
                schema: "dbo",
                table: "UserRole",
                columns: new[] { "UserId", "RoleId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserRole_UserId_RoleId",
                schema: "dbo",
                table: "UserRole");

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Role",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Role",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Role",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "UserRole",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "UserRole",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "UserRole",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "dbo",
                table: "UserRole");
        }
    }
}
#pragma warning restore CA1861 // Prefer 'static readonly' fields over constant array arguments
