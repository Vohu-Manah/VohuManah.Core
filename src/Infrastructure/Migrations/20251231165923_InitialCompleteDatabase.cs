using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 

namespace Infrastructure.Migrations
{
    
    public partial class InitialCompleteDatabase : Migration
    {
        private static readonly string[] LibrarianEndpoints = new[]
        {
            "Library.Books.Create", "Library.Books.Delete", "Library.Books.GetAll", "Library.Books.GetAllEntities",
            "Library.Books.GetById", "Library.Books.GetCount", "Library.Books.GetCountByLanguage", "Library.Books.GetCountByPublisher",
            "Library.Books.GetCountByPublishPlace", "Library.Books.GetCountBySubject", "Library.Books.GetCountByYear", "Library.Books.GetList",
            "Library.Books.GetStatistics", "Library.Books.GetVolumeCount", "Library.Books.Search", "Library.Books.Update", "Library.Books.GetProfile",
            "Library.Manuscripts.Create", "Library.Manuscripts.Delete", "Library.Manuscripts.GetAll", "Library.Manuscripts.GetAllEntities",
            "Library.Manuscripts.GetById", "Library.Manuscripts.GetCount", "Library.Manuscripts.GetCountByGap", "Library.Manuscripts.GetCountByLanguage",
            "Library.Manuscripts.GetCountBySubject", "Library.Manuscripts.GetCountByWritingPlace", "Library.Manuscripts.GetCountByYear", "Library.Manuscripts.GetList",
            "Library.Manuscripts.GetStatistics", "Library.Manuscripts.Search", "Library.Manuscripts.Update",
            "Library.Publications.Create", "Library.Publications.Delete", "Library.Publications.GetAll", "Library.Publications.GetAllEntities",
            "Library.Publications.GetById", "Library.Publications.GetCount", "Library.Publications.GetCountByLanguage", "Library.Publications.GetCountByPublishDate",
            "Library.Publications.GetCountByPublishPlace", "Library.Publications.GetCountBySubject", "Library.Publications.GetCountByType", "Library.Publications.GetList",
            "Library.Publications.GetStatistics", "Library.Publications.Search", "Library.Publications.Update",
            "Library.PublicationTypes.Create", "Library.PublicationTypes.Delete", "Library.PublicationTypes.GetAll", "Library.PublicationTypes.GetById",
            "Library.PublicationTypes.GetList", "Library.PublicationTypes.GetNames", "Library.PublicationTypes.Update",
            "Library.Publishers.Create", "Library.Publishers.Delete", "Library.Publishers.GetAll", "Library.Publishers.GetById",
            "Library.Publishers.GetList", "Library.Publishers.GetNames", "Library.Publishers.Update",
            "Library.Languages.Create", "Library.Languages.Delete", "Library.Languages.GetAll", "Library.Languages.GetById",
            "Library.Languages.GetList", "Library.Languages.GetNames", "Library.Languages.Update",
            "Library.Cities.Create", "Library.Cities.Delete", "Library.Cities.GetAll", "Library.Cities.GetById",
            "Library.Cities.GetList", "Library.Cities.GetNames", "Library.Cities.Update",
            "Library.Subjects.Create", "Library.Subjects.Delete", "Library.Subjects.GetAll", "Library.Subjects.GetById",
            "Library.Subjects.GetList", "Library.Subjects.GetNames", "Library.Subjects.Update",
            "Library.Gaps.Create", "Library.Gaps.Delete", "Library.Gaps.GetAll", "Library.Gaps.GetById",
            "Library.Gaps.GetList", "Library.Gaps.GetNames", "Library.Gaps.Update",
            "Library.Attachments.Upload", "Library.Attachments.Delete", "Library.Attachments.GetByEntity", "Library.Attachments.Update"
        };

        private static readonly string[] PublisherEndpoints = new[]
        {
            "Library.Publications.Create", "Library.Publications.Delete", "Library.Publications.GetAll", "Library.Publications.GetAllEntities",
            "Library.Publications.GetById", "Library.Publications.GetCount", "Library.Publications.GetCountByLanguage", "Library.Publications.GetCountByPublishDate",
            "Library.Publications.GetCountByPublishPlace", "Library.Publications.GetCountBySubject", "Library.Publications.GetCountByType", "Library.Publications.GetList",
            "Library.Publications.GetStatistics", "Library.Publications.Search", "Library.Publications.Update",
            "Library.Publishers.Create", "Library.Publishers.Delete", "Library.Publishers.GetAll", "Library.Publishers.GetById",
            "Library.Publishers.GetList", "Library.Publishers.GetNames", "Library.Publishers.Update",
            "Library.PublicationTypes.Create", "Library.PublicationTypes.Delete", "Library.PublicationTypes.GetAll", "Library.PublicationTypes.GetById",
            "Library.PublicationTypes.GetList", "Library.PublicationTypes.GetNames", "Library.PublicationTypes.Update"
        };

        private static readonly string[] AuthorEndpoints = new[]
        {
            "Library.Books.GetAll", "Library.Books.GetAllEntities", "Library.Books.GetById", "Library.Books.GetCount",
            "Library.Books.GetCountByLanguage", "Library.Books.GetCountByPublisher", "Library.Books.GetCountByPublishPlace",
            "Library.Books.GetCountBySubject", "Library.Books.GetCountByYear", "Library.Books.GetList", "Library.Books.GetStatistics",
            "Library.Books.GetVolumeCount", "Library.Books.Search", "Library.Books.GetProfile",
            "Library.Manuscripts.GetAll", "Library.Manuscripts.GetAllEntities", "Library.Manuscripts.GetById", "Library.Manuscripts.GetCount",
            "Library.Manuscripts.GetCountByGap", "Library.Manuscripts.GetCountByLanguage", "Library.Manuscripts.GetCountBySubject",
            "Library.Manuscripts.GetCountByWritingPlace", "Library.Manuscripts.GetCountByYear", "Library.Manuscripts.GetList",
            "Library.Manuscripts.GetStatistics", "Library.Manuscripts.Search",
            "Library.Publications.GetAll", "Library.Publications.GetAllEntities", "Library.Publications.GetById", "Library.Publications.GetCount",
            "Library.Publications.GetCountByLanguage", "Library.Publications.GetCountByPublishDate", "Library.Publications.GetCountByPublishPlace",
            "Library.Publications.GetCountBySubject", "Library.Publications.GetCountByType", "Library.Publications.GetList",
            "Library.Publications.GetStatistics", "Library.Publications.Search"
        };

        private static readonly string[] AdminEndpoints = new[]
        {
            "Library.Books.Create", "Library.Books.Delete", "Library.Books.GetAll", "Library.Books.GetAllEntities",
            "Library.Books.GetById", "Library.Books.GetCount", "Library.Books.GetCountByLanguage", "Library.Books.GetCountByPublisher",
            "Library.Books.GetCountByPublishPlace", "Library.Books.GetCountBySubject", "Library.Books.GetCountByYear", "Library.Books.GetList",
            "Library.Books.GetStatistics", "Library.Books.GetVolumeCount", "Library.Books.Search", "Library.Books.Update", "Library.Books.GetProfile",
            "Library.Cities.Create", "Library.Cities.Delete", "Library.Cities.GetAll", "Library.Cities.GetById",
            "Library.Cities.GetList", "Library.Cities.GetNames", "Library.Cities.Update",
            "Library.Gaps.Create", "Library.Gaps.Delete", "Library.Gaps.GetAll", "Library.Gaps.GetById",
            "Library.Gaps.GetList", "Library.Gaps.GetNames", "Library.Gaps.Update",
            "Library.Languages.Create", "Library.Languages.Delete", "Library.Languages.GetAll", "Library.Languages.GetById",
            "Library.Languages.GetList", "Library.Languages.GetNames", "Library.Languages.Update",
            "Library.Manuscripts.Create", "Library.Manuscripts.Delete", "Library.Manuscripts.GetAll", "Library.Manuscripts.GetAllEntities",
            "Library.Manuscripts.GetById", "Library.Manuscripts.GetCount", "Library.Manuscripts.GetCountByGap", "Library.Manuscripts.GetCountByLanguage",
            "Library.Manuscripts.GetCountBySubject", "Library.Manuscripts.GetCountByWritingPlace", "Library.Manuscripts.GetCountByYear", "Library.Manuscripts.GetList",
            "Library.Manuscripts.GetStatistics", "Library.Manuscripts.Search", "Library.Manuscripts.Update",
            "Library.Publications.Create", "Library.Publications.Delete", "Library.Publications.GetAll", "Library.Publications.GetAllEntities",
            "Library.Publications.GetById", "Library.Publications.GetCount", "Library.Publications.GetCountByLanguage", "Library.Publications.GetCountByPublishDate",
            "Library.Publications.GetCountByPublishPlace", "Library.Publications.GetCountBySubject", "Library.Publications.GetCountByType", "Library.Publications.GetList",
            "Library.Publications.GetStatistics", "Library.Publications.Search", "Library.Publications.Update",
            "Library.PublicationTypes.Create", "Library.PublicationTypes.Delete", "Library.PublicationTypes.GetAll", "Library.PublicationTypes.GetById",
            "Library.PublicationTypes.GetList", "Library.PublicationTypes.GetNames", "Library.PublicationTypes.Update",
            "Library.Publishers.Create", "Library.Publishers.Delete", "Library.Publishers.GetAll", "Library.Publishers.GetById",
            "Library.Publishers.GetList", "Library.Publishers.GetNames", "Library.Publishers.Update",
            "Library.Settings.AssignRoleToUser", "Library.Settings.CreateRole", "Library.Settings.GetAll", "Library.Settings.GetAllEntities",
            "Library.Settings.GetAllEndpoints", "Library.Settings.GetAllRoles", "Library.Settings.GetAppInfo", "Library.Settings.GetBackground", 
            "Library.Settings.GetById", "Library.Settings.GetByName", "Library.Settings.GetMainTitle", "Library.Settings.GetRolePermissions",
            "Library.Settings.GetSystemInfo", "Library.Settings.GetUserRoles", "Library.Settings.GetUserSettings", "Library.Settings.RemoveRoleFromUser", 
            "Library.Settings.Update", "Library.Settings.UpdateRoleEndpoints", "Library.Settings.UpdateUserSettings",
            "Library.Subjects.Create", "Library.Subjects.Delete", "Library.Subjects.GetAll", "Library.Subjects.GetById",
            "Library.Subjects.GetList", "Library.Subjects.GetNames", "Library.Subjects.Update",
            "Library.Users.ChangePassword", "Library.Users.Create", "Library.Users.Delete", "Library.Users.GetAll", "Library.Users.GetAllEntities",
            "Library.Users.GetByUserName", "Library.Users.GetFullName", "Library.Users.GetList", "Library.Users.Revoke", "Library.Users.Update",
            "Library.Users.UpdateProfile",
            "Library.Attachments.Upload", "Library.Attachments.Delete", "Library.Attachments.GetByEntity", "Library.Attachments.Update",
            "Library.Menu.GetAll", "Library.Menu.Create", "Library.Menu.Update", "Library.Menu.Delete", "Library.Menu.Reorder",
            "Reports.BookStatistics", "Reports.ManuscriptStatistics", "Reports.PublicationStatistics", "Reports.View", "Reports.Export",
            "Reports.BookSearchList", "Reports.ManuscriptSearchList", "Reports.PublicationSearchList"
        };

        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AppInfo]') AND type in (N'U'))
                BEGIN
                    CREATE TABLE [dbo].[AppInfo] (
                        [Id] int IDENTITY(1,1) NOT NULL,
                        [AppName] nvarchar(100) NOT NULL,
                        [Version] nvarchar(50) NOT NULL,
                        [Author] nvarchar(100) NOT NULL,
                        [Description] nvarchar(1000) NULL,
                        [LastUpdateDate] datetime2 NULL,
                        [Copyright] nvarchar(200) NULL,
                        [License] nvarchar(100) NULL,
                        [Website] nvarchar(200) NULL,
                        [SupportEmail] nvarchar(100) NULL,
                        CONSTRAINT [PK_AppInfo] PRIMARY KEY ([Id])
                    );
                END
            ");

            
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserSettings]') AND type in (N'U'))
                BEGIN
                    CREATE TABLE [dbo].[UserSettings] (
                        [Id] bigint IDENTITY(1,1) NOT NULL,
                        [UserId] bigint NOT NULL,
                        [ProfileImagePath] nvarchar(500) NULL,
                        [ThemePrimaryColor] nvarchar(50) NULL,
                        [ThemeSecondaryColor] nvarchar(50) NULL,
                        [ThemeAccentColor] nvarchar(50) NULL,
                        [ThemeMode] nvarchar(50) NULL,
                        CONSTRAINT [PK_UserSettings] PRIMARY KEY ([Id])
                    );
                END
            ");

            
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserSettings]') AND type in (N'U'))
                AND NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_UserSettings_LibraryUsers_UserId')
                AND EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LibraryUsers]') AND type in (N'U'))
                BEGIN
                    ALTER TABLE [dbo].[UserSettings]
                    ADD CONSTRAINT [FK_UserSettings_LibraryUsers_UserId]
                    FOREIGN KEY ([UserId]) REFERENCES [dbo].[LibraryUsers] ([Id]) ON DELETE CASCADE;
                END
            ");

            
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserSettings]') AND type in (N'U'))
                AND NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_UserSettings_UserId' AND object_id = OBJECT_ID(N'[dbo].[UserSettings]'))
                BEGIN
                    CREATE INDEX [IX_UserSettings_UserId] ON [dbo].[UserSettings] ([UserId]);
                END
            ");

            
            SeedDataIfNotExists(migrationBuilder);
        }

        private static void SeedDataIfNotExists(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM [dbo].[City])
                BEGIN
                    SET IDENTITY_INSERT [dbo].[City] ON;
                    INSERT INTO [dbo].[City] ([Id], [Name]) VALUES
                        (1, N'تهران'),
                        (2, N'اصفهان'),
                        (3, N'شیراز');
                    SET IDENTITY_INSERT [dbo].[City] OFF;
                END
            ");

            
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM [dbo].[Language])
                BEGIN
                    SET IDENTITY_INSERT [dbo].[Language] ON;
                    INSERT INTO [dbo].[Language] ([Id], [Name], [Abbreviation]) VALUES
                        (1, N'فارسی', N'FA'),
                        (2, N'English', N'EN'),
                        (3, N'العربية', N'AR');
                    SET IDENTITY_INSERT [dbo].[Language] OFF;
                END
            ");

            
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM [dbo].[Subject])
                BEGIN
                    SET IDENTITY_INSERT [dbo].[Subject] ON;
                    INSERT INTO [dbo].[Subject] ([Id], [Title]) VALUES
                        (1, N'ادبیات'),
                        (2, N'تاریخ'),
                        (3, N'فلسفه'),
                        (4, N'علوم');
                    SET IDENTITY_INSERT [dbo].[Subject] OFF;
                END
            ");

            
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM [dbo].[PublicationType])
                BEGIN
                    SET IDENTITY_INSERT [dbo].[PublicationType] ON;
                    INSERT INTO [dbo].[PublicationType] ([Id], [Title]) VALUES
                        (1, N'مجله'),
                        (2, N'روزنامه'),
                        (3, N'نشریه');
                    SET IDENTITY_INSERT [dbo].[PublicationType] OFF;
                END
            ");

            
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM [dbo].[Gap])
                BEGIN
                    SET IDENTITY_INSERT [dbo].[Gap] ON;
                    INSERT INTO [dbo].[Gap] ([Id], [Title], [State], [Note]) VALUES
                        (1, N'گپ 1', 1, N'توضیحات گپ 1'),
                        (2, N'گپ 2', 0, N'توضیحات گپ 2');
                    SET IDENTITY_INSERT [dbo].[Gap] OFF;
                END
            ");

            
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM [dbo].[Settings])
                BEGIN
                    SET IDENTITY_INSERT [dbo].[Settings] ON;
                    INSERT INTO [dbo].[Settings] ([Id], [Name], [Value]) VALUES
                        (1, N'BackgroundImageFolder', N'D:\Image'),
                        (2, N'CurrentBackgroundImageName', N'library.jpg'),
                        (3, N'MainTitle', N'فرم اصلی');
                    SET IDENTITY_INSERT [dbo].[Settings] OFF;
                END
            ");

            
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM [dbo].[AppInfo])
                BEGIN
                    SET IDENTITY_INSERT [dbo].[AppInfo] ON;
                    INSERT INTO [dbo].[AppInfo] ([Id], [AppName], [Version], [Author], [Description], [LastUpdateDate], [Copyright], [License], [Website], [SupportEmail]) VALUES
                        (1, N'Vohu Manah', N'1.0.0', N'Vohu Manah Team', N'سیستم مدیریت کتابخانه', GETUTCDATE(), N'© 2025 Vohu Manah', N'Proprietary', N'https://vohumanah.com', N'support@vohumanah.com');
                    SET IDENTITY_INSERT [dbo].[AppInfo] OFF;
                END
            ");

            
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM [dbo].[Publisher])
                AND EXISTS (SELECT 1 FROM [dbo].[City] WHERE [Id] = 1)
                BEGIN
                    SET IDENTITY_INSERT [dbo].[Publisher] ON;
                    INSERT INTO [dbo].[Publisher] ([Id], [Name], [ManagerName], [PlaceId], [Address], [Telephone], [Website], [Email]) VALUES
                        (1, N'انتشارات امیرکبیر', N'مدیر انتشارات', 1, N'تهران، خیابان انقلاب', N'021-12345678', N'www.amirkabir.com', N'info@amirkabir.com');
                    SET IDENTITY_INSERT [dbo].[Publisher] OFF;
                END
            ");

            
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM [dbo].[Role])
                BEGIN
                    SET IDENTITY_INSERT [dbo].[Role] ON;
                    INSERT INTO [dbo].[Role] ([Id], [Name]) VALUES
                        (1, N'Admin'),
                        (2, N'User'),
                        (3, N'Librarian'),
                        (4, N'Publisher'),
                        (5, N'Author');
                    SET IDENTITY_INSERT [dbo].[Role] OFF;
                END
            ");

            
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM [dbo].[LibraryUsers] WHERE [UserName] = N'admin')
                BEGIN
                    INSERT INTO [dbo].[LibraryUsers] ([UserName], [Password], [Name], [LastName]) VALUES
                        (N'admin', N'9843417E11AFA30B39EB2E974DC2DCB2E53D4B608E665E478A1686C065107225-71815962DF6849A69BE50976ECA3DA44', N'admin', N'admin');
                END
            ");

            
            migrationBuilder.Sql(@"
                DECLARE @AdminUserId BIGINT = (SELECT [Id] FROM [dbo].[LibraryUsers] WHERE [UserName] = N'admin');
                IF @AdminUserId IS NOT NULL
                AND NOT EXISTS (SELECT 1 FROM [dbo].[UserRole] WHERE [UserId] = @AdminUserId)
                AND EXISTS (SELECT 1 FROM [dbo].[Role] WHERE [Id] IN (1, 2, 3, 4, 5))
                BEGIN
                    SET IDENTITY_INSERT [dbo].[UserRole] ON;
                    INSERT INTO [dbo].[UserRole] ([Id], [UserId], [UserName], [RoleId]) VALUES
                        (1, @AdminUserId, N'admin', 1),
                        (2, @AdminUserId, N'admin', 2),
                        (3, @AdminUserId, N'admin', 3),
                        (4, @AdminUserId, N'admin', 4),
                        (5, @AdminUserId, N'admin', 5);
                    SET IDENTITY_INSERT [dbo].[UserRole] OFF;
                END
            ");

            
            SeedRoleEndpoints(migrationBuilder, "Admin", AdminEndpoints);
            SeedRoleEndpoints(migrationBuilder, "Librarian", LibrarianEndpoints);
            SeedRoleEndpoints(migrationBuilder, "Publisher", PublisherEndpoints);
            SeedRoleEndpoints(migrationBuilder, "Author", AuthorEndpoints);
        }

        private static void SeedRoleEndpoints(MigrationBuilder migrationBuilder, string roleName, string[] endpoints)
        {
            if (endpoints == null || endpoints.Length == 0)
                return;

            string endpointValues = string.Join(",\n", endpoints.Select(ep => $"('{ep.Replace("'", "''")}')"));

            string sql = $@"
DECLARE @RoleId INT = (SELECT [Id] FROM [dbo].[Role] WHERE [Name] = '{roleName.Replace("'", "''")}');
IF @RoleId IS NOT NULL
BEGIN
    INSERT INTO [dbo].[RolePermission] ([RoleId], [EndpointName])
    SELECT @RoleId, ep.EndpointName
    FROM (VALUES {endpointValues}) AS ep(EndpointName)
    WHERE NOT EXISTS (
        SELECT 1 FROM [dbo].[RolePermission]
        WHERE [RoleId] = @RoleId AND [EndpointName] = ep.EndpointName
    );
END";

            migrationBuilder.Sql(sql);
        }

        
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppInfo",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "UserSettings",
                schema: "dbo");
        }
    }
}
