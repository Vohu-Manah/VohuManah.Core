using Application.Library.Settings;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedDefaultRolePermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            SeedRoleEndpoints(migrationBuilder, RolePermissionDefaults.Librarian, RolePermissionDefaults.LibrarianEndpoints);
            SeedRoleEndpoints(migrationBuilder, RolePermissionDefaults.Publisher, RolePermissionDefaults.PublisherEndpoints);
            SeedRoleEndpoints(migrationBuilder, RolePermissionDefaults.Author, RolePermissionDefaults.AuthorEndpoints);

            // Ensure Admin role has access to the new role-management endpoints
            var adminEndpoints = new[]
            {
                "Library.Settings.CreateRole",
                "Library.Settings.UpdateRoleEndpoints"
            };

            SeedRoleEndpoints(migrationBuilder, RolePermissionDefaults.Admin, adminEndpoints);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            RemoveRoleEndpoints(migrationBuilder, RolePermissionDefaults.Librarian);
            RemoveRoleEndpoints(migrationBuilder, RolePermissionDefaults.Publisher);
            RemoveRoleEndpoints(migrationBuilder, RolePermissionDefaults.Author);

            var adminEndpoints = new[]
            {
                "Library.Settings.CreateRole",
                "Library.Settings.UpdateRoleEndpoints"
            };

            RemoveSpecificEndpointsFromRole(migrationBuilder, RolePermissionDefaults.Admin, adminEndpoints);
        }

        private static void SeedRoleEndpoints(MigrationBuilder migrationBuilder, string roleName, IEnumerable<string> endpoints)
        {
            string endpointValues = string.Join(
                ",\n",
                endpoints.Select(ep => $"('{ep}')"));

            if (string.IsNullOrWhiteSpace(endpointValues))
            {
                return;
            }

            string sql = $@"
DECLARE @RoleId INT = (SELECT Id FROM [dbo].[Role] WHERE [Name] = '{roleName}');
IF @RoleId IS NOT NULL
BEGIN
    INSERT INTO [dbo].[RolePermission] (RoleId, EndpointName)
    SELECT @RoleId, ep.EndpointName
    FROM (VALUES {endpointValues}) AS ep(EndpointName)
    WHERE NOT EXISTS (
        SELECT 1 FROM [dbo].[RolePermission]
        WHERE RoleId = @RoleId AND EndpointName = ep.EndpointName
    );
END";

            migrationBuilder.Sql(sql);
        }

        private static void RemoveRoleEndpoints(MigrationBuilder migrationBuilder, string roleName)
        {
            string sql = $@"
DECLARE @RoleId INT = (SELECT Id FROM [dbo].[Role] WHERE [Name] = '{roleName}');
IF @RoleId IS NOT NULL
BEGIN
    DELETE FROM [dbo].[RolePermission]
    WHERE RoleId = @RoleId;
END";

            migrationBuilder.Sql(sql);
        }

        private static void RemoveSpecificEndpointsFromRole(MigrationBuilder migrationBuilder, string roleName, IEnumerable<string> endpoints)
        {
            string endpointValues = string.Join(
                ",",
                endpoints.Select(ep => $"'{ep}'"));

            if (string.IsNullOrWhiteSpace(endpointValues))
            {
                return;
            }

            string sql = $@"
DECLARE @RoleId INT = (SELECT Id FROM [dbo].[Role] WHERE [Name] = '{roleName}');
IF @RoleId IS NOT NULL
BEGIN
    DELETE FROM [dbo].[RolePermission]
    WHERE RoleId = @RoleId AND EndpointName IN ({endpointValues});
END";

            migrationBuilder.Sql(sql);
        }
    }
}
