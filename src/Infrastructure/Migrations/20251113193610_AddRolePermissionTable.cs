using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable
#pragma warning disable CA1861 // Prefer 'static readonly' fields over constant array arguments

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRolePermissionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RolePermission",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    EndpointName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolePermission_Role_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "dbo",
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "LibraryUsers",
                keyColumn: "Id",
                keyValue: 1L,
                column: "Password",
                value: "F5DBDB150DB276F90D7B9660461ABA07D1B8AB0364242AD52FEAA88B3680DAC3-BB19FC50EAE2BA0F64974B3911FBD6EB");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermission_RoleId_EndpointName",
                schema: "dbo",
                table: "RolePermission",
                columns: new[] { "RoleId", "EndpointName" },
                unique: true);
        }
#pragma warning restore CA1861 // Prefer 'static readonly' fields over constant array arguments

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RolePermission",
                schema: "dbo");

            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "LibraryUsers",
                keyColumn: "Id",
                keyValue: 1L,
                column: "Password",
                value: "9843417E11AFA30B39EB2E974DC2DCB2E53D4B608E665E478A1686C065107225-71815962DF6849A69BE50976ECA3DA44");
        }
    }
}
