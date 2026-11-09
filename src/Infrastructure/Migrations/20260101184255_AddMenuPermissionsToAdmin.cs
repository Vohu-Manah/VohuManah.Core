using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    
    public partial class AddMenuPermissionsToAdmin : Migration
    {
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DECLARE @AdminRoleId INT = (SELECT [Id] FROM [dbo].[Role] WHERE [Name] = N'Admin');
                
                IF @AdminRoleId IS NOT NULL
                BEGIN
                    INSERT INTO [dbo].[RolePermission] ([RoleId], [EndpointName])
                    SELECT @AdminRoleId, ep.EndpointName
                    FROM (VALUES 
                        ('Library.Menu.GetAll'),
                        ('Library.Menu.Create'),
                        ('Library.Menu.Update'),
                        ('Library.Menu.Delete'),
                        ('Library.Menu.Reorder')
                    ) AS ep(EndpointName)
                    WHERE NOT EXISTS (
                        SELECT 1 FROM [dbo].[RolePermission]
                        WHERE [RoleId] = @AdminRoleId AND [EndpointName] = ep.EndpointName
                    );
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
                    AND [EndpointName] IN (
                        'Library.Menu.GetAll',
                        'Library.Menu.Create',
                        'Library.Menu.Update',
                        'Library.Menu.Delete',
                        'Library.Menu.Reorder'
                    );
                END
            ");
        }
    }
}
