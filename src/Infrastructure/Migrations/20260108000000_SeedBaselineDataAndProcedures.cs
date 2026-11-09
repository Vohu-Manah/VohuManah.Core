using Infrastructure.Database;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VohuManah.Infrastructure.Database.Migrations
{
    
    
    
    
    
    
    public partial class SeedBaselineDataAndProcedures : Migration
    {
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
-- ============================================================
-- مایگریشن داده‌های پایه و پروسیجرها
-- نقش‌ها، شهر/زبان/موضوع، کاربر admin، مجوزها، منو و پروسیجرهای بکاپ/نگهداری
-- ============================================================

SET NOCOUNT ON;
SET XACT_ABORT ON;

-- ============ داده‌های پایه ============

IF NOT EXISTS (SELECT 1 FROM [dbo].[City] WHERE 1 = 1)
BEGIN
    SET IDENTITY_INSERT [dbo].[City] ON;
    INSERT INTO [dbo].[City] ([Id], [Name]) VALUES
    (1, N'تهران'),
    (2, N'اصفهان'),
    (3, N'شیراز'),
    (4, N'مشهد');
    SET IDENTITY_INSERT [dbo].[City] OFF;
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Language] WHERE 1 = 1)
BEGIN
    SET IDENTITY_INSERT [dbo].[Language] ON;
    INSERT INTO [dbo].[Language] ([Id], [Abbreviation], [Name]) VALUES
    (1, N'FA', N'فارسی'),
    (2, N'EN', N'English'),
    (3, N'AR', N'العربية');
    SET IDENTITY_INSERT [dbo].[Language] OFF;
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Subject] WHERE 1 = 1)
BEGIN
    SET IDENTITY_INSERT [dbo].[Subject] ON;
    INSERT INTO [dbo].[Subject] ([Id], [Title]) VALUES
    (1, N'ادبیات'),
    (2, N'تاریخ'),
    (3, N'فلسفه'),
    (4, N'علوم'),
    (5, N'ریاضی');
    SET IDENTITY_INSERT [dbo].[Subject] OFF;
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[PublicationType] WHERE 1 = 1)
BEGIN
    SET IDENTITY_INSERT [dbo].[PublicationType] ON;
    INSERT INTO [dbo].[PublicationType] ([Id], [Title]) VALUES
    (1, N'مجله'),
    (2, N'روزنامه'),
    (3, N'نشریه'),
    (4, N'مجلهٔ علمی'),
    (5, N'مجلهٔ ادبی');
    SET IDENTITY_INSERT [dbo].[PublicationType] OFF;
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Gap] WHERE 1 = 1)
BEGIN
    SET IDENTITY_INSERT [dbo].[Gap] ON;
    INSERT INTO [dbo].[Gap] ([Id], [Note], [State], [Title]) VALUES
    (1, N'توضیحات گپ 1', 1, N'گپ 1'),
    (2, N'توضیحات گپ 2', 0, N'گپ 2'),
    (3, N'یادداشت', 1, N'فاصلهٔ ۱'),
    (4, N'یادداشت', 1, N'فاصلهٔ ۲');
    SET IDENTITY_INSERT [dbo].[Gap] OFF;
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Role] WHERE 1 = 1)
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

-- ============ کاربر پیش‌فرض admin (رمز: admin123) ============
IF NOT EXISTS (SELECT 1 FROM [dbo].[LibraryUsers] WHERE [UserName] = N'admin')
BEGIN
    SET IDENTITY_INSERT [dbo].[LibraryUsers] ON;
    INSERT INTO [dbo].[LibraryUsers] ([Id], [LastName], [Name], [Password], [UserName])
    VALUES (1, N'admin', N'admin', N'14986C70D50819BB0B679F9286541B74F9F506F5E6747FBD7844CE10CDB47396-54E6838894EF267BE8C0DD4F94618197', N'admin');
    SET IDENTITY_INSERT [dbo].[LibraryUsers] OFF;
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[UserRole] WHERE 1 = 1)
BEGIN
    SET IDENTITY_INSERT [dbo].[UserRole] ON;
    INSERT INTO [dbo].[UserRole] ([Id], [RoleId], [UserId], [UserName]) VALUES
    (1, 1, 1, N'admin'),
    (2, 2, 1, N'admin'),
    (3, 3, 1, N'admin'),
    (4, 4, 1, N'admin'),
    (5, 5, 1, N'admin');
    SET IDENTITY_INSERT [dbo].[UserRole] OFF;
END


-- ============ مجوزهای نقش‌ها (302 مورد) ============
IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE 1 = 1)
BEGIN
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Books.Create')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Books.Create');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Books.Delete')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Books.Delete');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Books.GetAll')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Books.GetAll');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Books.GetAllEntities')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Books.GetAllEntities');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Books.GetById')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Books.GetById');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Books.GetCount')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Books.GetCount');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Books.GetCountByLanguage')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Books.GetCountByLanguage');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Books.GetCountByPublisher')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Books.GetCountByPublisher');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Books.GetCountByPublishPlace')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Books.GetCountByPublishPlace');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Books.GetCountBySubject')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Books.GetCountBySubject');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Books.GetCountByYear')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Books.GetCountByYear');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Books.GetList')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Books.GetList');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Books.GetStatistics')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Books.GetStatistics');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Books.GetVolumeCount')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Books.GetVolumeCount');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Books.Search')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Books.Search');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Books.Update')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Books.Update');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Books.GetProfile')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Books.GetProfile');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Cities.Create')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Cities.Create');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Cities.Delete')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Cities.Delete');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Cities.GetAll')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Cities.GetAll');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Cities.GetById')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Cities.GetById');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Cities.GetList')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Cities.GetList');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Cities.GetNames')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Cities.GetNames');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Cities.Update')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Cities.Update');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Gaps.Create')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Gaps.Create');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Gaps.Delete')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Gaps.Delete');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Gaps.GetAll')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Gaps.GetAll');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Gaps.GetById')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Gaps.GetById');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Gaps.GetList')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Gaps.GetList');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Gaps.GetNames')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Gaps.GetNames');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Gaps.Update')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Gaps.Update');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Languages.Create')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Languages.Create');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Languages.Delete')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Languages.Delete');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Languages.GetAll')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Languages.GetAll');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Languages.GetById')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Languages.GetById');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Languages.GetList')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Languages.GetList');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Languages.GetNames')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Languages.GetNames');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Languages.Update')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Languages.Update');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Manuscripts.Create')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Manuscripts.Create');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Manuscripts.Delete')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Manuscripts.Delete');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Manuscripts.GetAll')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Manuscripts.GetAll');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Manuscripts.GetAllEntities')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Manuscripts.GetAllEntities');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Manuscripts.GetById')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Manuscripts.GetById');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Manuscripts.GetCount')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Manuscripts.GetCount');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Manuscripts.GetCountByGap')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Manuscripts.GetCountByGap');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Manuscripts.GetCountByLanguage')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Manuscripts.GetCountByLanguage');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Manuscripts.GetCountBySubject')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Manuscripts.GetCountBySubject');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Manuscripts.GetCountByWritingPlace')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Manuscripts.GetCountByWritingPlace');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Manuscripts.GetCountByYear')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Manuscripts.GetCountByYear');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Manuscripts.GetList')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Manuscripts.GetList');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Manuscripts.GetStatistics')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Manuscripts.GetStatistics');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Manuscripts.Search')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Manuscripts.Search');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Manuscripts.Update')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Manuscripts.Update');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Publications.Create')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Publications.Create');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Publications.Delete')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Publications.Delete');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Publications.GetAll')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Publications.GetAll');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Publications.GetAllEntities')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Publications.GetAllEntities');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Publications.GetById')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Publications.GetById');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Publications.GetCount')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Publications.GetCount');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Publications.GetCountByLanguage')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Publications.GetCountByLanguage');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Publications.GetCountByPublishDate')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Publications.GetCountByPublishDate');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Publications.GetCountByPublishPlace')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Publications.GetCountByPublishPlace');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Publications.GetCountBySubject')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Publications.GetCountBySubject');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Publications.GetCountByType')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Publications.GetCountByType');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Publications.GetList')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Publications.GetList');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Publications.GetStatistics')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Publications.GetStatistics');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Publications.Search')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Publications.Search');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Publications.Update')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Publications.Update');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.PublicationTypes.Create')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.PublicationTypes.Create');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.PublicationTypes.Delete')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.PublicationTypes.Delete');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.PublicationTypes.GetAll')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.PublicationTypes.GetAll');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.PublicationTypes.GetById')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.PublicationTypes.GetById');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.PublicationTypes.GetList')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.PublicationTypes.GetList');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.PublicationTypes.GetNames')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.PublicationTypes.GetNames');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.PublicationTypes.Update')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.PublicationTypes.Update');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Publishers.Create')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Publishers.Create');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Publishers.Delete')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Publishers.Delete');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Publishers.GetAll')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Publishers.GetAll');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Publishers.GetById')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Publishers.GetById');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Publishers.GetList')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Publishers.GetList');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Publishers.GetNames')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Publishers.GetNames');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Publishers.Update')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Publishers.Update');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Settings.AssignRoleToUser')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Settings.AssignRoleToUser');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Settings.CreateRole')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Settings.CreateRole');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Settings.GetAll')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Settings.GetAll');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Settings.GetAllEntities')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Settings.GetAllEntities');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Settings.GetAllEndpoints')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Settings.GetAllEndpoints');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Settings.GetAllRoles')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Settings.GetAllRoles');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Settings.GetAppInfo')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Settings.GetAppInfo');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Settings.GetBackground')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Settings.GetBackground');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Settings.GetById')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Settings.GetById');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Settings.GetByName')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Settings.GetByName');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Settings.GetMainTitle')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Settings.GetMainTitle');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Settings.GetSystemInfo')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Settings.GetSystemInfo');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Settings.GetUserRoles')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Settings.GetUserRoles');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Settings.GetUserSettings')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Settings.GetUserSettings');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Settings.RemoveRoleFromUser')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Settings.RemoveRoleFromUser');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Settings.Update')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Settings.Update');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Settings.UpdateRoleEndpoints')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Settings.UpdateRoleEndpoints');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Settings.UpdateUserSettings')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Settings.UpdateUserSettings');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Subjects.Create')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Subjects.Create');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Subjects.Delete')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Subjects.Delete');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Subjects.GetAll')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Subjects.GetAll');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Subjects.GetById')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Subjects.GetById');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Subjects.GetList')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Subjects.GetList');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Subjects.GetNames')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Subjects.GetNames');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Subjects.Update')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Subjects.Update');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Users.ChangePassword')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Users.ChangePassword');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Users.Create')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Users.Create');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Users.Delete')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Users.Delete');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Users.GetAll')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Users.GetAll');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Users.GetAllEntities')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Users.GetAllEntities');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Users.GetByUserName')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Users.GetByUserName');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Users.GetFullName')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Users.GetFullName');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Users.GetList')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Users.GetList');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Users.Revoke')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Users.Revoke');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Users.Update')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Users.Update');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Users.UpdateProfile')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Users.UpdateProfile');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Attachments.Upload')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Attachments.Upload');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Attachments.Delete')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Attachments.Delete');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Attachments.GetByEntity')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Attachments.GetByEntity');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Attachments.Update')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Attachments.Update');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Reports.BookStatistics')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Reports.BookStatistics');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Reports.ManuscriptStatistics')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Reports.ManuscriptStatistics');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Reports.PublicationStatistics')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Reports.PublicationStatistics');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Reports.View')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Reports.View');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Reports.Export')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Reports.Export');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Reports.BookSearchList')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Reports.BookSearchList');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Reports.ManuscriptSearchList')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Reports.ManuscriptSearchList');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Reports.PublicationSearchList')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Reports.PublicationSearchList');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Attachments.Delete')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Attachments.Delete');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Attachments.GetByEntity')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Attachments.GetByEntity');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Attachments.Update')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Attachments.Update');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Attachments.Upload')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Attachments.Upload');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Books.Create')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Books.Create');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Books.Delete')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Books.Delete');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Books.GetAll')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Books.GetAll');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Books.GetAllEntities')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Books.GetAllEntities');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Books.GetById')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Books.GetById');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Books.GetCount')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Books.GetCount');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Books.GetCountByLanguage')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Books.GetCountByLanguage');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Books.GetCountByPublisher')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Books.GetCountByPublisher');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Books.GetCountByPublishPlace')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Books.GetCountByPublishPlace');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Books.GetCountBySubject')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Books.GetCountBySubject');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Books.GetCountByYear')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Books.GetCountByYear');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Books.GetList')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Books.GetList');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Books.GetProfile')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Books.GetProfile');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Books.GetStatistics')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Books.GetStatistics');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Books.GetVolumeCount')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Books.GetVolumeCount');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Books.Search')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Books.Search');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Books.Update')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Books.Update');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Cities.Create')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Cities.Create');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Cities.Delete')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Cities.Delete');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Cities.GetAll')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Cities.GetAll');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Cities.GetById')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Cities.GetById');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Cities.GetList')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Cities.GetList');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Cities.GetNames')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Cities.GetNames');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Cities.Update')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Cities.Update');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Gaps.Create')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Gaps.Create');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Gaps.Delete')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Gaps.Delete');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Gaps.GetAll')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Gaps.GetAll');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Gaps.GetById')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Gaps.GetById');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Gaps.GetList')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Gaps.GetList');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Gaps.GetNames')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Gaps.GetNames');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Gaps.Update')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Gaps.Update');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Languages.Create')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Languages.Create');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Languages.Delete')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Languages.Delete');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Languages.GetAll')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Languages.GetAll');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Languages.GetById')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Languages.GetById');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Languages.GetList')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Languages.GetList');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Languages.GetNames')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Languages.GetNames');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Languages.Update')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Languages.Update');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Manuscripts.Create')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Manuscripts.Create');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Manuscripts.Delete')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Manuscripts.Delete');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Manuscripts.GetAll')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Manuscripts.GetAll');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Manuscripts.GetAllEntities')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Manuscripts.GetAllEntities');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Manuscripts.GetById')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Manuscripts.GetById');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Manuscripts.GetCount')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Manuscripts.GetCount');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Manuscripts.GetCountByGap')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Manuscripts.GetCountByGap');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Manuscripts.GetCountByLanguage')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Manuscripts.GetCountByLanguage');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Manuscripts.GetCountBySubject')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Manuscripts.GetCountBySubject');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Manuscripts.GetCountByWritingPlace')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Manuscripts.GetCountByWritingPlace');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Manuscripts.GetCountByYear')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Manuscripts.GetCountByYear');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Manuscripts.GetList')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Manuscripts.GetList');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Manuscripts.GetStatistics')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Manuscripts.GetStatistics');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Manuscripts.Search')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Manuscripts.Search');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Manuscripts.Update')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Manuscripts.Update');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Publications.Create')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Publications.Create');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Publications.Delete')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Publications.Delete');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Publications.GetAll')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Publications.GetAll');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Publications.GetAllEntities')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Publications.GetAllEntities');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Publications.GetById')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Publications.GetById');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Publications.GetCount')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Publications.GetCount');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Publications.GetCountByLanguage')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Publications.GetCountByLanguage');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Publications.GetCountByPublishDate')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Publications.GetCountByPublishDate');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Publications.GetCountByPublishPlace')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Publications.GetCountByPublishPlace');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Publications.GetCountBySubject')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Publications.GetCountBySubject');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Publications.GetCountByType')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Publications.GetCountByType');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Publications.GetList')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Publications.GetList');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Publications.GetStatistics')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Publications.GetStatistics');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Publications.Search')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Publications.Search');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Publications.Update')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Publications.Update');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.PublicationTypes.Create')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.PublicationTypes.Create');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.PublicationTypes.Delete')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.PublicationTypes.Delete');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.PublicationTypes.GetAll')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.PublicationTypes.GetAll');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.PublicationTypes.GetById')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.PublicationTypes.GetById');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.PublicationTypes.GetList')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.PublicationTypes.GetList');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.PublicationTypes.GetNames')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.PublicationTypes.GetNames');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.PublicationTypes.Update')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.PublicationTypes.Update');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Publishers.Create')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Publishers.Create');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Publishers.Delete')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Publishers.Delete');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Publishers.GetAll')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Publishers.GetAll');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Publishers.GetById')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Publishers.GetById');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Publishers.GetList')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Publishers.GetList');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Publishers.GetNames')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Publishers.GetNames');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Publishers.Update')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Publishers.Update');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Subjects.Create')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Subjects.Create');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Subjects.Delete')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Subjects.Delete');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Subjects.GetAll')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Subjects.GetAll');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Subjects.GetById')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Subjects.GetById');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Subjects.GetList')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Subjects.GetList');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Subjects.GetNames')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Subjects.GetNames');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 3 AND [EndpointName] = N'Library.Subjects.Update')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (3, N'Library.Subjects.Update');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 4 AND [EndpointName] = N'Library.Publications.Create')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (4, N'Library.Publications.Create');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 4 AND [EndpointName] = N'Library.Publications.Delete')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (4, N'Library.Publications.Delete');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 4 AND [EndpointName] = N'Library.Publications.GetAll')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (4, N'Library.Publications.GetAll');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 4 AND [EndpointName] = N'Library.Publications.GetAllEntities')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (4, N'Library.Publications.GetAllEntities');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 4 AND [EndpointName] = N'Library.Publications.GetById')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (4, N'Library.Publications.GetById');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 4 AND [EndpointName] = N'Library.Publications.GetCount')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (4, N'Library.Publications.GetCount');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 4 AND [EndpointName] = N'Library.Publications.GetCountByLanguage')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (4, N'Library.Publications.GetCountByLanguage');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 4 AND [EndpointName] = N'Library.Publications.GetCountByPublishDate')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (4, N'Library.Publications.GetCountByPublishDate');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 4 AND [EndpointName] = N'Library.Publications.GetCountByPublishPlace')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (4, N'Library.Publications.GetCountByPublishPlace');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 4 AND [EndpointName] = N'Library.Publications.GetCountBySubject')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (4, N'Library.Publications.GetCountBySubject');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 4 AND [EndpointName] = N'Library.Publications.GetCountByType')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (4, N'Library.Publications.GetCountByType');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 4 AND [EndpointName] = N'Library.Publications.GetList')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (4, N'Library.Publications.GetList');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 4 AND [EndpointName] = N'Library.Publications.GetStatistics')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (4, N'Library.Publications.GetStatistics');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 4 AND [EndpointName] = N'Library.Publications.Search')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (4, N'Library.Publications.Search');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 4 AND [EndpointName] = N'Library.Publications.Update')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (4, N'Library.Publications.Update');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 4 AND [EndpointName] = N'Library.Publishers.Create')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (4, N'Library.Publishers.Create');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 4 AND [EndpointName] = N'Library.Publishers.Delete')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (4, N'Library.Publishers.Delete');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 4 AND [EndpointName] = N'Library.Publishers.GetAll')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (4, N'Library.Publishers.GetAll');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 4 AND [EndpointName] = N'Library.Publishers.GetById')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (4, N'Library.Publishers.GetById');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 4 AND [EndpointName] = N'Library.Publishers.GetList')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (4, N'Library.Publishers.GetList');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 4 AND [EndpointName] = N'Library.Publishers.GetNames')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (4, N'Library.Publishers.GetNames');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 4 AND [EndpointName] = N'Library.Publishers.Update')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (4, N'Library.Publishers.Update');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 4 AND [EndpointName] = N'Library.PublicationTypes.Create')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (4, N'Library.PublicationTypes.Create');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 4 AND [EndpointName] = N'Library.PublicationTypes.Delete')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (4, N'Library.PublicationTypes.Delete');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 4 AND [EndpointName] = N'Library.PublicationTypes.GetAll')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (4, N'Library.PublicationTypes.GetAll');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 4 AND [EndpointName] = N'Library.PublicationTypes.GetById')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (4, N'Library.PublicationTypes.GetById');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 4 AND [EndpointName] = N'Library.PublicationTypes.GetList')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (4, N'Library.PublicationTypes.GetList');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 4 AND [EndpointName] = N'Library.PublicationTypes.GetNames')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (4, N'Library.PublicationTypes.GetNames');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 4 AND [EndpointName] = N'Library.PublicationTypes.Update')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (4, N'Library.PublicationTypes.Update');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 5 AND [EndpointName] = N'Library.Books.GetAll')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (5, N'Library.Books.GetAll');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 5 AND [EndpointName] = N'Library.Books.GetAllEntities')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (5, N'Library.Books.GetAllEntities');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 5 AND [EndpointName] = N'Library.Books.GetById')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (5, N'Library.Books.GetById');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 5 AND [EndpointName] = N'Library.Books.GetCount')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (5, N'Library.Books.GetCount');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 5 AND [EndpointName] = N'Library.Books.GetCountByLanguage')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (5, N'Library.Books.GetCountByLanguage');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 5 AND [EndpointName] = N'Library.Books.GetCountByPublisher')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (5, N'Library.Books.GetCountByPublisher');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 5 AND [EndpointName] = N'Library.Books.GetCountByPublishPlace')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (5, N'Library.Books.GetCountByPublishPlace');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 5 AND [EndpointName] = N'Library.Books.GetCountBySubject')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (5, N'Library.Books.GetCountBySubject');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 5 AND [EndpointName] = N'Library.Books.GetCountByYear')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (5, N'Library.Books.GetCountByYear');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 5 AND [EndpointName] = N'Library.Books.GetList')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (5, N'Library.Books.GetList');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 5 AND [EndpointName] = N'Library.Books.GetStatistics')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (5, N'Library.Books.GetStatistics');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 5 AND [EndpointName] = N'Library.Books.GetVolumeCount')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (5, N'Library.Books.GetVolumeCount');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 5 AND [EndpointName] = N'Library.Books.Search')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (5, N'Library.Books.Search');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 5 AND [EndpointName] = N'Library.Books.GetProfile')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (5, N'Library.Books.GetProfile');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 5 AND [EndpointName] = N'Library.Manuscripts.GetAll')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (5, N'Library.Manuscripts.GetAll');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 5 AND [EndpointName] = N'Library.Manuscripts.GetAllEntities')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (5, N'Library.Manuscripts.GetAllEntities');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 5 AND [EndpointName] = N'Library.Manuscripts.GetById')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (5, N'Library.Manuscripts.GetById');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 5 AND [EndpointName] = N'Library.Manuscripts.GetCount')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (5, N'Library.Manuscripts.GetCount');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 5 AND [EndpointName] = N'Library.Manuscripts.GetCountByGap')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (5, N'Library.Manuscripts.GetCountByGap');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 5 AND [EndpointName] = N'Library.Manuscripts.GetCountByLanguage')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (5, N'Library.Manuscripts.GetCountByLanguage');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 5 AND [EndpointName] = N'Library.Manuscripts.GetCountBySubject')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (5, N'Library.Manuscripts.GetCountBySubject');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 5 AND [EndpointName] = N'Library.Manuscripts.GetCountByWritingPlace')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (5, N'Library.Manuscripts.GetCountByWritingPlace');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 5 AND [EndpointName] = N'Library.Manuscripts.GetCountByYear')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (5, N'Library.Manuscripts.GetCountByYear');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 5 AND [EndpointName] = N'Library.Manuscripts.GetList')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (5, N'Library.Manuscripts.GetList');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 5 AND [EndpointName] = N'Library.Manuscripts.GetStatistics')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (5, N'Library.Manuscripts.GetStatistics');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 5 AND [EndpointName] = N'Library.Manuscripts.Search')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (5, N'Library.Manuscripts.Search');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 5 AND [EndpointName] = N'Library.Publications.GetAll')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (5, N'Library.Publications.GetAll');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 5 AND [EndpointName] = N'Library.Publications.GetAllEntities')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (5, N'Library.Publications.GetAllEntities');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 5 AND [EndpointName] = N'Library.Publications.GetById')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (5, N'Library.Publications.GetById');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 5 AND [EndpointName] = N'Library.Publications.GetCount')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (5, N'Library.Publications.GetCount');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 5 AND [EndpointName] = N'Library.Publications.GetCountByLanguage')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (5, N'Library.Publications.GetCountByLanguage');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 5 AND [EndpointName] = N'Library.Publications.GetCountByPublishDate')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (5, N'Library.Publications.GetCountByPublishDate');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 5 AND [EndpointName] = N'Library.Publications.GetCountByPublishPlace')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (5, N'Library.Publications.GetCountByPublishPlace');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 5 AND [EndpointName] = N'Library.Publications.GetCountBySubject')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (5, N'Library.Publications.GetCountBySubject');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 5 AND [EndpointName] = N'Library.Publications.GetCountByType')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (5, N'Library.Publications.GetCountByType');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 5 AND [EndpointName] = N'Library.Publications.GetList')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (5, N'Library.Publications.GetList');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 5 AND [EndpointName] = N'Library.Publications.GetStatistics')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (5, N'Library.Publications.GetStatistics');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 5 AND [EndpointName] = N'Library.Publications.Search')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (5, N'Library.Publications.Search');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Menu.GetAll')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Menu.GetAll');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Menu.Create')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Menu.Create');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Menu.Update')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Menu.Update');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Menu.Delete')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Menu.Delete');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Menu.Reorder')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Menu.Reorder');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Settings.GetRolePermissions')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Settings.GetRolePermissions');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Menu.ResetToDefault')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Menu.ResetToDefault');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Visits.GetStats')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Visits.GetStats');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Backup.GetInfo')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Backup.GetInfo');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Backup.Create')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Backup.Create');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Backup.Download')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Backup.Download');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Backup.Restore')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Backup.Restore');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermission] WHERE [RoleId] = 1 AND [EndpointName] = N'Library.Backup.Cancel')
        INSERT INTO [dbo].[RolePermission] ([RoleId],[EndpointName]) VALUES (1, N'Library.Backup.Cancel');
END

-- ============ منو (36 مورد) ============
IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuItem] WHERE 1 = 1)
BEGIN
    SET IDENTITY_INSERT [dbo].[MenuItem] ON;
    INSERT INTO [dbo].[MenuItem] ([Id],[Label],[Path],[Icon],[ParentId],[DisplayOrder],[IsActive],[IsSystem]) VALUES
    (37, N'داشبورد', N'/', N'FaHome', NULL, 0, 1, 0),
    (38, N'کتابها', NULL, N'FaBook', NULL, 1, 1, 0),
    (39, N'مدیریت کتابها', N'/books', NULL, 38, 0, 1, 0),
    (40, N'جستجوی کتاب', N'/books/search', NULL, 38, 1, 1, 0),
    (41, N'آمار کتابها', N'/books/statistics', NULL, 38, 2, 1, 0),
    (42, N'نشریات', NULL, N'FaNewspaper', NULL, 2, 1, 0),
    (43, N'مدیریت نشریات', N'/publications', NULL, 42, 0, 1, 0),
    (44, N'جستجوی نشریه', N'/publications/search', NULL, 42, 1, 1, 0),
    (45, N'آمار نشریات', N'/publications/statistics', NULL, 42, 2, 1, 0),
    (46, N'نسخ خطی', NULL, N'FaScroll', NULL, 3, 1, 0),
    (47, N'مدیریت نسخ خطی', N'/manuscripts', NULL, 46, 0, 1, 0),
    (48, N'جستجوی نسخه خطی', N'/manuscripts/search', NULL, 46, 1, 1, 0),
    (49, N'آمار نسخ خطی', N'/manuscripts/statistics', NULL, 46, 2, 1, 0),
    (50, N'اطلاعات پایه', NULL, N'FaTags', NULL, 4, 1, 0),
    (51, N'شهرها', N'/cities', NULL, 50, 0, 1, 0),
    (52, N'زبانها', N'/languages', NULL, 50, 1, 1, 0),
    (53, N'موضوعات', N'/subjects', NULL, 50, 2, 1, 0),
    (54, N'ناشران', N'/publishers', NULL, 50, 3, 1, 0),
    (55, N'فاصلهها', N'/gaps', NULL, 50, 4, 1, 0),
    (56, N'انواع نشریات', N'/publication-types', NULL, 50, 5, 1, 0),
    (57, N'کاربران', NULL, N'FaUsers', NULL, 5, 1, 0),
    (58, N'مدیریت کاربران', N'/users', NULL, 57, 0, 1, 0),
    (59, N'پروفایل', N'/profile', N'FaUser', NULL, 6, 1, 0),
    (60, N'گزارشها', NULL, N'FaFileAlt', NULL, 7, 1, 0),
    (61, N'جستجوی کتاب', N'/books/search', NULL, 60, 0, 1, 0),
    (62, N'جستجوی نسخه خطی', N'/manuscripts/search', NULL, 60, 1, 1, 0),
    (63, N'جستجوی نشریه', N'/publications/search', NULL, 60, 2, 1, 0),
    (64, N'گزارشات آماری کتاب', N'/books/statistics', NULL, 60, 3, 1, 0),
    (65, N'گزارشات آماری نسخ خطی', N'/manuscripts/statistics', NULL, 60, 4, 1, 0),
    (66, N'گزارشات آماری نشریه', N'/publications/statistics', NULL, 60, 5, 1, 0),
    (67, N'تنظیمات', NULL, N'FaCog', NULL, 8, 1, 0),
    (68, N'تنظیمات عمومی', N'/settings', NULL, 67, 0, 1, 0),
    (69, N'تنظیمات منو', N'/menu-settings', NULL, 67, 1, 1, 1),
    (70, N'مدیریت ادمین', NULL, N'FaShieldAlt', NULL, 9, 1, 0),
    (71, N'مدیریت نقشها', N'/admin/roles', NULL, 70, 0, 1, 0),
    (72, N'نقشدهی به کاربران', N'/admin/user-roles', NULL, 70, 1, 1, 0);
    SET IDENTITY_INSERT [dbo].[MenuItem] OFF;
END

-- ============ اطلاعات برنامه ============

-- ============ تنظیمات پایه ============
IF NOT EXISTS (SELECT 1 FROM [dbo].[Settings] WHERE 1 = 1)
BEGIN
    SET IDENTITY_INSERT [dbo].[Settings] ON;
    INSERT INTO [dbo].[Settings] ([Id],[Name],[Value]) VALUES
    (1, N'BackgroundImageFolder', N'D:\Image'),
    (2, N'CurrentBackgroundImageName', N'library.jpg'),
    (3, N'MainTitle', N'فرم اصلی');
    SET IDENTITY_INSERT [dbo].[Settings] OFF;
END

-- پروسیجر بکاپ
EXEC(N'CREATE OR ALTER PROCEDURE [dbo].[usp_BackupDatabase]
                @BackupPath NVARCHAR(500)
            AS
            BEGIN
                SET NOCOUNT ON;

                -- نام دیتابیس از context فعلی خوانده میشود تا بین محیطهای dev/production تکراری نباشد
                DECLARE @db NVARCHAR(128) = DB_NAME();
                DECLARE @sqlBackup NVARCHAR(MAX) =
                    N''BACKUP DATABASE '' + QUOTENAME(@db) +
                    N'' TO DISK = '' + QUOTENAME(@BackupPath, '''''''') +
                    N'' WITH INIT, STATS = 10'';

                EXEC (@sqlBackup);
            END')

-- پروسیجر نگهداری دیتابیس
EXEC(N'CREATE OR ALTER PROCEDURE dbo.usp_MaintainDatabase
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @dbName sysname = DB_NAME();
    DECLARE @logFileId int = (SELECT file_id FROM sys.database_files WHERE type = 2);
    DECLARE @dataFileId int = (SELECT file_id FROM sys.database_files WHERE type = 1);
    DECLARE @logSizeBefore bigint, @dataSizeBefore bigint;
    DECLARE @logSizeAfter bigint, @dataSizeAfter bigint;
    DECLARE @freedLog bigint, @freedData bigint;
    DECLARE @rebuilt int = 0;

    SELECT @logSizeBefore = size FROM sys.database_files WHERE file_id = @logFileId;
    SELECT @dataSizeBefore = size FROM sys.database_files WHERE file_id = @dataFileId;

    -- ۱) بازسازی ایندکس‌های پرfragmentation (فقط > 20% و حداقل 1000 صفحه)
    --    این کار هم جستجوها را تند می‌کند هم فضای آزاد می‌سازد که shrink آن را می‌گیرد
    DECLARE @sqlMaint nvarchar(max) = N'''';

    SELECT @sqlMaint = @sqlMaint + N''ALTER INDEX '' + QUOTENAME(i.name) + N'' ON '' + QUOTENAME(s.name) + N'' REBUILD;''
    FROM sys.dm_db_index_physical_stats(DB_ID(), NULL, NULL, NULL, ''LIMITED'') ps
    JOIN sys.indexes i ON ps.object_id = i.object_id AND ps.index_id = i.index_id
    JOIN sys.tables s ON i.object_id = s.object_id
    WHERE ps.page_count >= 1000
      AND ps.avg_fragmentation_in_percent > 20
      AND i.name IS NOT NULL;

    IF LEN(@sqlMaint) > 0
    BEGIN
        EXEC sp_executesql @sqlMaint;
        SELECT @rebuilt = @@ROWCOUNT;
    END

    -- ۲) آزادسازی فضای لاگ (SIMPLE recovery ⇒ truncate log)
    IF EXISTS (SELECT 1 FROM sys.databases WHERE name = @dbName AND recovery_model_desc = ''SIMPLE'')
    BEGIN
        CHECKPOINT;
        BEGIN TRY
            DBCC SHRINKFILE (@logFileId, 64); -- حداقل 64MB
        END TRY
        BEGIN CATCH
            -- اگر shrink ممکن نبود (لاگ در حال استفاده) بی‌خیال
        END CATCH
    END

    -- ۳) shrink دیتا فقط اگر فضای آزاد واقعی داشته باشیم (DBCC SHRINKFILE فایل را کوچک می‌کند نه محتوا را)
    BEGIN TRY
        DBCC SHRINKFILE (@dataFileId, 0); -- 0 = تا حد ممکن کوچک کن
    END TRY
    BEGIN CATCH
    END CATCH

    SELECT @logSizeAfter = size FROM sys.database_files WHERE file_id = @logFileId;
    SELECT @dataSizeAfter = size FROM sys.database_files WHERE file_id = @dataFileId;
    SET @freedLog = @logSizeBefore - @logSizeAfter;
    SET @freedData = @dataSizeBefore - @dataSizeAfter;

    -- ۴) بازگرداندن autogrowth معقول (data: 10% , log: 64MB)
    --    نام فایل باید ثابت باشد (ALTER DATABASE با متغیر پشتیبانی نمی‌شود)
    IF DB_NAME() = ''Library''
    BEGIN
        IF EXISTS (SELECT 1 FROM sys.database_files WHERE name = ''Library'' AND is_percent_growth = 0)
            ALTER DATABASE [Library] MODIFY FILE (NAME = N''Library'', FILEGROWTH = 10%);
        IF EXISTS (SELECT 1 FROM sys.database_files WHERE name = ''Library_log'' AND is_percent_growth = 0)
            ALTER DATABASE [Library] MODIFY FILE (NAME = N''Library_log'', FILEGROWTH = 64MB);
    END

    SELECT
        @rebuilt AS indexes_rebuilt,
        CAST(@freedLog / 128.0 AS DECIMAL(18,2)) AS freed_log_mb,
        CAST(@freedData / 128.0 AS DECIMAL(18,2)) AS freed_data_mb,
        CAST(@dataSizeAfter / 128.0 / 1024 AS DECIMAL(18,2)) AS data_size_gb,
        CAST((SELECT SUM(CAST(size AS BIGINT)) * 8.0 / 1024 / 1024 FROM sys.database_files) AS DECIMAL(18,2)) AS total_size_gb,
        (SELECT recovery_model_desc FROM sys.databases WHERE name = @dbName) AS recovery_model,
        (SELECT COUNT(*) FROM sys.dm_db_index_physical_stats(DB_ID(), NULL, NULL, NULL, ''LIMITED'') WHERE page_count >= 1000 AND avg_fragmentation_in_percent > 20) AS remaining_fragmented;
END
')
");
        }

        
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            throw new NotSupportedException();
        }
    }
}
