using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    
    public partial class EnsureMenuPermissionsAdded : Migration
    {
        
        protected override void Up(MigrationBuilder migrationBuilder)
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
                    
                    INSERT INTO [dbo].[RolePermission] ([RoleId], [EndpointName])
                    VALUES
                        (@AdminRoleId, 'Library.Menu.GetAll'),
                        (@AdminRoleId, 'Library.Menu.Create'),
                        (@AdminRoleId, 'Library.Menu.Update'),
                        (@AdminRoleId, 'Library.Menu.Delete'),
                        (@AdminRoleId, 'Library.Menu.Reorder');
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
