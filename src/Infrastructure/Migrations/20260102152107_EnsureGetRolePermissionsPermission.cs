using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    
#pragma warning disable CA1711 
    public partial class EnsureGetRolePermissionsPermission : Migration
#pragma warning restore CA1711
    {
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DECLARE @AdminRoleId INT = (SELECT [Id] FROM [dbo].[Role] WHERE [Name] = N'Admin');
                
                IF @AdminRoleId IS NOT NULL
                BEGIN
                    DELETE FROM [dbo].[RolePermission]
                    WHERE [RoleId] = @AdminRoleId 
                    AND [EndpointName] = 'Library.Settings.GetRolePermissions';
                    
                    INSERT INTO [dbo].[RolePermission] ([RoleId], [EndpointName])
                    VALUES (@AdminRoleId, 'Library.Settings.GetRolePermissions');
                END
            ");
        }

        
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DECLARE @AdminRoleId INT = (SELECT [Id] FROM [dbo].[Role] WHERE [Name] = N'Admin');
                
                IF @AdminRoleId IS NOT NULL
                BEGIN
                    DELETE FROM [dbo].[RolePermission]
                    WHERE [RoleId] = @AdminRoleId 
                    AND [EndpointName] = 'Library.Settings.GetRolePermissions';
                END
            ");
        }
    }
}
