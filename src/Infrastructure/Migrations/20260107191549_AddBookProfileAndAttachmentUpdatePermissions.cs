using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    
    public partial class AddBookProfileAndAttachmentUpdatePermissions : Migration
    {
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                -- Add Library.Books.GetProfile to Admin, Librarian, and Author roles
                DECLARE @AdminRoleId INT = (SELECT [Id] FROM [dbo].[Role] WHERE [Name] = N'Admin');
                DECLARE @LibrarianRoleId INT = (SELECT [Id] FROM [dbo].[Role] WHERE [Name] = N'Librarian');
                DECLARE @AuthorRoleId INT = (SELECT [Id] FROM [dbo].[Role] WHERE [Name] = N'Author');
                
                -- Add Library.Books.GetProfile to Admin
                IF @AdminRoleId IS NOT NULL
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = @AdminRoleId AND [EndpointName] = N'Library.Books.GetProfile')
                    BEGIN
                        INSERT INTO [dbo].[RolePermission] ([RoleId], [EndpointName]) VALUES (@AdminRoleId, N'Library.Books.GetProfile');
                    END
                END
                
                -- Add Library.Books.GetProfile to Librarian
                IF @LibrarianRoleId IS NOT NULL
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = @LibrarianRoleId AND [EndpointName] = N'Library.Books.GetProfile')
                    BEGIN
                        INSERT INTO [dbo].[RolePermission] ([RoleId], [EndpointName]) VALUES (@LibrarianRoleId, N'Library.Books.GetProfile');
                    END
                END
                
                -- Add Library.Books.GetProfile to Author
                IF @AuthorRoleId IS NOT NULL
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = @AuthorRoleId AND [EndpointName] = N'Library.Books.GetProfile')
                    BEGIN
                        INSERT INTO [dbo].[RolePermission] ([RoleId], [EndpointName]) VALUES (@AuthorRoleId, N'Library.Books.GetProfile');
                    END
                END
                
                -- Add Library.Attachments.Update to Admin
                IF @AdminRoleId IS NOT NULL
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = @AdminRoleId AND [EndpointName] = N'Library.Attachments.Update')
                    BEGIN
                        INSERT INTO [dbo].[RolePermission] ([RoleId], [EndpointName]) VALUES (@AdminRoleId, N'Library.Attachments.Update');
                    END
                END
                
                -- Add Library.Attachments.Update to Librarian
                IF @LibrarianRoleId IS NOT NULL
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = @LibrarianRoleId AND [EndpointName] = N'Library.Attachments.Update')
                    BEGIN
                        INSERT INTO [dbo].[RolePermission] ([RoleId], [EndpointName]) VALUES (@LibrarianRoleId, N'Library.Attachments.Update');
                    END
                END
            ");
        }

        
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                -- Remove Library.Books.GetProfile and Library.Attachments.Update from all roles
                DECLARE @AdminRoleId INT = (SELECT [Id] FROM [dbo].[Role] WHERE [Name] = N'Admin');
                DECLARE @LibrarianRoleId INT = (SELECT [Id] FROM [dbo].[Role] WHERE [Name] = N'Librarian');
                DECLARE @AuthorRoleId INT = (SELECT [Id] FROM [dbo].[Role] WHERE [Name] = N'Author');
                
                -- Remove Library.Books.GetProfile from Admin
                IF @AdminRoleId IS NOT NULL
                BEGIN
                    DELETE FROM [dbo].[RolePermission] WHERE [RoleId] = @AdminRoleId AND [EndpointName] = N'Library.Books.GetProfile';
                END
                
                -- Remove Library.Books.GetProfile from Librarian
                IF @LibrarianRoleId IS NOT NULL
                BEGIN
                    DELETE FROM [dbo].[RolePermission] WHERE [RoleId] = @LibrarianRoleId AND [EndpointName] = N'Library.Books.GetProfile';
                END
                
                -- Remove Library.Books.GetProfile from Author
                IF @AuthorRoleId IS NOT NULL
                BEGIN
                    DELETE FROM [dbo].[RolePermission] WHERE [RoleId] = @AuthorRoleId AND [EndpointName] = N'Library.Books.GetProfile';
                END
                
                -- Remove Library.Attachments.Update from Admin
                IF @AdminRoleId IS NOT NULL
                BEGIN
                    DELETE FROM [dbo].[RolePermission] WHERE [RoleId] = @AdminRoleId AND [EndpointName] = N'Library.Attachments.Update';
                END
                
                -- Remove Library.Attachments.Update from Librarian
                IF @LibrarianRoleId IS NOT NULL
                BEGIN
                    DELETE FROM [dbo].[RolePermission] WHERE [RoleId] = @LibrarianRoleId AND [EndpointName] = N'Library.Attachments.Update';
                END
            ");
        }
    }
}
