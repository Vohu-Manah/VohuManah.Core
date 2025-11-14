using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EnsureAdminUserExists : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM [dbo].[Role] WHERE [Name] = 'Admin')
                BEGIN
                    INSERT INTO [dbo].[Role] ([Name]) VALUES ('Admin');
                END
                
                IF NOT EXISTS (SELECT 1 FROM [dbo].[Role] WHERE [Name] = 'User')
                BEGIN
                    INSERT INTO [dbo].[Role] ([Name]) VALUES ('User');
                END
                
                IF NOT EXISTS (SELECT 1 FROM [dbo].[LibraryUsers] WHERE [UserName] = 'admin')
                BEGIN
                    INSERT INTO [dbo].[LibraryUsers] ([Id], [UserName], [Password], [Name], [LastName])
                    VALUES (1, 'admin', '9843417E11AFA30B39EB2E974DC2DCB2E53D4B608E665E478A1686C065107225-71815962DF6849A69BE50976ECA3DA44', 'admin', 'admin');
                END
                ELSE
                BEGIN
                    UPDATE [dbo].[LibraryUsers]
                    SET [Password] = '9843417E11AFA30B39EB2E974DC2DCB2E53D4B608E665E478A1686C065107225-71815962DF6849A69BE50976ECA3DA44'
                    WHERE [UserName] = 'admin';
                END
                
                DECLARE @AdminRoleId INT = (SELECT Id FROM [dbo].[Role] WHERE [Name] = 'Admin');
                DECLARE @UserRoleId INT = (SELECT Id FROM [dbo].[Role] WHERE [Name] = 'User');
                
                IF @AdminRoleId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM [dbo].[UserRole] WHERE [UserName] = 'admin' AND [RoleId] = @AdminRoleId)
                BEGIN
                    INSERT INTO [dbo].[UserRole] ([UserName], [RoleId])
                    VALUES ('admin', @AdminRoleId);
                END
                
                IF @UserRoleId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM [dbo].[UserRole] WHERE [UserName] = 'admin' AND [RoleId] = @UserRoleId)
                BEGIN
                    INSERT INTO [dbo].[UserRole] ([UserName], [RoleId])
                    VALUES ('admin', @UserRoleId);
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE [dbo].[LibraryUsers]
                SET [Password] = '648E9E14DB5F5EBC7B5E3892EE6E5B4C7C49A233DBDAC3C169C33708CC95EF44-65DF715A949CBD1C8FE829424B5FD78E'
                WHERE [UserName] = 'admin';
            ");
        }
    }
}
