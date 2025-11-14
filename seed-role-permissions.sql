-- Script SQL برای اضافه کردن همه endpointهای Library به نقش Admin
-- این script باید بعد از ایجاد جدول RolePermission اجرا شود

-- گرفتن شناسه نقش Admin
DECLARE @AdminRoleId INT = (SELECT Id FROM [dbo].[Role] WHERE [Name] = 'Admin');

-- اگر نقش Admin وجود نداشت، خطا می‌دهد
IF @AdminRoleId IS NULL
BEGIN
    RAISERROR('نقش Admin یافت نشد!', 16, 1);
    RETURN;
END

-- اضافه کردن همه endpointها به نقش Admin
-- استفاده از MERGE برای جلوگیری از duplicate entries
MERGE [dbo].[RolePermission] AS target
USING (VALUES
    -- Books
    ('Library.Books.Create'),
    ('Library.Books.Update'),
    ('Library.Books.Delete'),
    ('Library.Books.GetById'),
    ('Library.Books.GetAll'),
    ('Library.Books.GetList'),
    ('Library.Books.GetAllEntities'),
    ('Library.Books.GetCount'),
    ('Library.Books.GetCountByLanguage'),
    ('Library.Books.GetCountByPublisher'),
    ('Library.Books.GetCountByPublishPlace'),
    ('Library.Books.GetCountBySubject'),
    ('Library.Books.GetCountByYear'),
    ('Library.Books.GetStatistics'),
    ('Library.Books.GetVolumeCount'),
    ('Library.Books.Search'),
    
    -- Cities
    ('Library.Cities.Create'),
    ('Library.Cities.Update'),
    ('Library.Cities.Delete'),
    ('Library.Cities.GetById'),
    ('Library.Cities.GetAll'),
    ('Library.Cities.GetList'),
    ('Library.Cities.GetNames'),
    
    -- Gaps
    ('Library.Gaps.Create'),
    ('Library.Gaps.Update'),
    ('Library.Gaps.Delete'),
    ('Library.Gaps.GetById'),
    ('Library.Gaps.GetAll'),
    ('Library.Gaps.GetList'),
    ('Library.Gaps.GetNames'),
    
    -- Languages
    ('Library.Languages.Create'),
    ('Library.Languages.Update'),
    ('Library.Languages.Delete'),
    ('Library.Languages.GetById'),
    ('Library.Languages.GetAll'),
    ('Library.Languages.GetList'),
    ('Library.Languages.GetNames'),
    
    -- Manuscripts
    ('Library.Manuscripts.Create'),
    ('Library.Manuscripts.Update'),
    ('Library.Manuscripts.Delete'),
    ('Library.Manuscripts.GetById'),
    ('Library.Manuscripts.GetAll'),
    ('Library.Manuscripts.GetList'),
    ('Library.Manuscripts.GetAllEntities'),
    ('Library.Manuscripts.GetCount'),
    ('Library.Manuscripts.GetCountByGap'),
    ('Library.Manuscripts.GetCountByLanguage'),
    ('Library.Manuscripts.GetCountBySubject'),
    ('Library.Manuscripts.GetCountByWritingPlace'),
    ('Library.Manuscripts.GetCountByYear'),
    ('Library.Manuscripts.GetStatistics'),
    ('Library.Manuscripts.Search'),
    
    -- Publications
    ('Library.Publications.Create'),
    ('Library.Publications.Update'),
    ('Library.Publications.Delete'),
    ('Library.Publications.GetById'),
    ('Library.Publications.GetAll'),
    ('Library.Publications.GetList'),
    ('Library.Publications.GetAllEntities'),
    ('Library.Publications.GetCount'),
    ('Library.Publications.GetCountByLanguage'),
    ('Library.Publications.GetCountByPublishDate'),
    ('Library.Publications.GetCountByPublishPlace'),
    ('Library.Publications.GetCountBySubject'),
    ('Library.Publications.GetCountByType'),
    ('Library.Publications.GetStatistics'),
    ('Library.Publications.Search'),
    
    -- PublicationTypes
    ('Library.PublicationTypes.Create'),
    ('Library.PublicationTypes.Update'),
    ('Library.PublicationTypes.Delete'),
    ('Library.PublicationTypes.GetById'),
    ('Library.PublicationTypes.GetAll'),
    ('Library.PublicationTypes.GetList'),
    ('Library.PublicationTypes.GetNames'),
    
    -- Publishers
    ('Library.Publishers.Create'),
    ('Library.Publishers.Update'),
    ('Library.Publishers.Delete'),
    ('Library.Publishers.GetById'),
    ('Library.Publishers.GetAll'),
    ('Library.Publishers.GetList'),
    ('Library.Publishers.GetNames'),
    
    -- Settings
    ('Library.Settings.Update'),
    ('Library.Settings.GetById'),
    ('Library.Settings.GetAll'),
    ('Library.Settings.GetAllEntities'),
    ('Library.Settings.GetByName'),
    ('Library.Settings.GetBackground'),
    ('Library.Settings.GetMainTitle'),
    ('Library.Settings.GetAllRoles'),
    ('Library.Settings.GetUserRoles'),
    ('Library.Settings.AssignRoleToUser'),
    ('Library.Settings.RemoveRoleFromUser'),
    
    -- Subjects
    ('Library.Subjects.Create'),
    ('Library.Subjects.Update'),
    ('Library.Subjects.Delete'),
    ('Library.Subjects.GetById'),
    ('Library.Subjects.GetAll'),
    ('Library.Subjects.GetList'),
    ('Library.Subjects.GetNames'),
    
    -- Users
    ('Library.Users.Create'),
    ('Library.Users.Update'),
    ('Library.Users.Delete'),
    ('Library.Users.GetAll'),
    ('Library.Users.GetList'),
    ('Library.Users.GetAllEntities'),
    ('Library.Users.GetByUserName'),
    ('Library.Users.GetFullName'),
    ('Library.Users.Revoke')
) AS source (EndpointName)
ON target.RoleId = @AdminRoleId AND target.EndpointName = source.EndpointName
WHEN NOT MATCHED THEN
    INSERT (RoleId, EndpointName)
    VALUES (@AdminRoleId, source.EndpointName);

-- نمایش تعداد endpointهای اضافه شده
SELECT 
    COUNT(*) AS TotalEndpoints,
    'Admin' AS RoleName
FROM [dbo].[RolePermission]
WHERE RoleId = @AdminRoleId;

PRINT 'همه endpointها به نقش Admin اضافه شدند!';

