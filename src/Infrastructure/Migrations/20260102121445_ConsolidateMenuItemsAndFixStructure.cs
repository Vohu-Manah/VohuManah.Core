using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    
    public partial class ConsolidateMenuItemsAndFixStructure : Migration
    {
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                -- غیرفعال کردن موقت constraint برای حذف
                ALTER TABLE [dbo].[MenuItem] NOCHECK CONSTRAINT [FK_MenuItem_MenuItem_ParentId];
                
                -- حذف تمام منوهای غیرسیستمی
                DELETE FROM [dbo].[MenuItem] WHERE [IsSystem] = 0;
                
                -- فعال کردن مجدد constraint
                ALTER TABLE [dbo].[MenuItem] CHECK CONSTRAINT [FK_MenuItem_MenuItem_ParentId];
                
                -- Seed مجدد منوهای استاتیک با ساختار صحیح
                -- داشبورد
                IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuItem] WHERE [Label] = N'داشبورد' AND [Path] = N'/')
                BEGIN
                    INSERT INTO [dbo].[MenuItem] ([Label], [Path], [Icon], [ParentId], [DisplayOrder], [IsActive], [IsSystem])
                    VALUES (N'داشبورد', N'/', N'FaHome', NULL, 0, 1, 0);
                END
                
                -- کتاب‌ها
                IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuItem] WHERE [Label] = N'کتاب‌ها' AND [ParentId] IS NULL)
                BEGIN
                    DECLARE @BooksParentId INT;
                    INSERT INTO [dbo].[MenuItem] ([Label], [Path], [Icon], [ParentId], [DisplayOrder], [IsActive], [IsSystem])
                    VALUES (N'کتاب‌ها', NULL, N'FaBook', NULL, 1, 1, 0);
                    SET @BooksParentId = SCOPE_IDENTITY();
                    
                    INSERT INTO [dbo].[MenuItem] ([Label], [Path], [Icon], [ParentId], [DisplayOrder], [IsActive], [IsSystem])
                    VALUES (N'مدیریت کتاب‌ها', N'/books', NULL, @BooksParentId, 0, 1, 0);
                    
                    INSERT INTO [dbo].[MenuItem] ([Label], [Path], [Icon], [ParentId], [DisplayOrder], [IsActive], [IsSystem])
                    VALUES (N'جستجوی کتاب', N'/books/search', NULL, @BooksParentId, 1, 1, 0);
                    
                    INSERT INTO [dbo].[MenuItem] ([Label], [Path], [Icon], [ParentId], [DisplayOrder], [IsActive], [IsSystem])
                    VALUES (N'آمار کتاب‌ها', N'/books/statistics', NULL, @BooksParentId, 2, 1, 0);
                END
                
                -- نشریات
                IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuItem] WHERE [Label] = N'نشریات' AND [ParentId] IS NULL)
                BEGIN
                    DECLARE @PublicationsParentId INT;
                    INSERT INTO [dbo].[MenuItem] ([Label], [Path], [Icon], [ParentId], [DisplayOrder], [IsActive], [IsSystem])
                    VALUES (N'نشریات', NULL, N'FaNewspaper', NULL, 2, 1, 0);
                    SET @PublicationsParentId = SCOPE_IDENTITY();
                    
                    INSERT INTO [dbo].[MenuItem] ([Label], [Path], [Icon], [ParentId], [DisplayOrder], [IsActive], [IsSystem])
                    VALUES (N'مدیریت نشریات', N'/publications', NULL, @PublicationsParentId, 0, 1, 0);
                    
                    INSERT INTO [dbo].[MenuItem] ([Label], [Path], [Icon], [ParentId], [DisplayOrder], [IsActive], [IsSystem])
                    VALUES (N'جستجوی نشریه', N'/publications/search', NULL, @PublicationsParentId, 1, 1, 0);
                    
                    INSERT INTO [dbo].[MenuItem] ([Label], [Path], [Icon], [ParentId], [DisplayOrder], [IsActive], [IsSystem])
                    VALUES (N'آمار نشریات', N'/publications/statistics', NULL, @PublicationsParentId, 2, 1, 0);
                END
                
                -- نسخ خطی
                IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuItem] WHERE [Label] = N'نسخ خطی' AND [ParentId] IS NULL)
                BEGIN
                    DECLARE @ManuscriptsParentId INT;
                    INSERT INTO [dbo].[MenuItem] ([Label], [Path], [Icon], [ParentId], [DisplayOrder], [IsActive], [IsSystem])
                    VALUES (N'نسخ خطی', NULL, N'FaScroll', NULL, 3, 1, 0);
                    SET @ManuscriptsParentId = SCOPE_IDENTITY();
                    
                    INSERT INTO [dbo].[MenuItem] ([Label], [Path], [Icon], [ParentId], [DisplayOrder], [IsActive], [IsSystem])
                    VALUES (N'مدیریت نسخ خطی', N'/manuscripts', NULL, @ManuscriptsParentId, 0, 1, 0);
                    
                    INSERT INTO [dbo].[MenuItem] ([Label], [Path], [Icon], [ParentId], [DisplayOrder], [IsActive], [IsSystem])
                    VALUES (N'جستجوی نسخه خطی', N'/manuscripts/search', NULL, @ManuscriptsParentId, 1, 1, 0);
                    
                    INSERT INTO [dbo].[MenuItem] ([Label], [Path], [Icon], [ParentId], [DisplayOrder], [IsActive], [IsSystem])
                    VALUES (N'آمار نسخ خطی', N'/manuscripts/statistics', NULL, @ManuscriptsParentId, 2, 1, 0);
                END
                
                -- اطلاعات پایه
                IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuItem] WHERE [Label] = N'اطلاعات پایه' AND [ParentId] IS NULL)
                BEGIN
                    DECLARE @BaseInfoParentId INT;
                    INSERT INTO [dbo].[MenuItem] ([Label], [Path], [Icon], [ParentId], [DisplayOrder], [IsActive], [IsSystem])
                    VALUES (N'اطلاعات پایه', NULL, N'FaTags', NULL, 4, 1, 0);
                    SET @BaseInfoParentId = SCOPE_IDENTITY();
                    
                    INSERT INTO [dbo].[MenuItem] ([Label], [Path], [Icon], [ParentId], [DisplayOrder], [IsActive], [IsSystem])
                    VALUES (N'شهرها', N'/cities', NULL, @BaseInfoParentId, 0, 1, 0);
                    
                    INSERT INTO [dbo].[MenuItem] ([Label], [Path], [Icon], [ParentId], [DisplayOrder], [IsActive], [IsSystem])
                    VALUES (N'زبان‌ها', N'/languages', NULL, @BaseInfoParentId, 1, 1, 0);
                    
                    INSERT INTO [dbo].[MenuItem] ([Label], [Path], [Icon], [ParentId], [DisplayOrder], [IsActive], [IsSystem])
                    VALUES (N'موضوعات', N'/subjects', NULL, @BaseInfoParentId, 2, 1, 0);
                    
                    INSERT INTO [dbo].[MenuItem] ([Label], [Path], [Icon], [ParentId], [DisplayOrder], [IsActive], [IsSystem])
                    VALUES (N'ناشران', N'/publishers', NULL, @BaseInfoParentId, 3, 1, 0);
                    
                    INSERT INTO [dbo].[MenuItem] ([Label], [Path], [Icon], [ParentId], [DisplayOrder], [IsActive], [IsSystem])
                    VALUES (N'فاصله‌ها', N'/gaps', NULL, @BaseInfoParentId, 4, 1, 0);
                    
                    INSERT INTO [dbo].[MenuItem] ([Label], [Path], [Icon], [ParentId], [DisplayOrder], [IsActive], [IsSystem])
                    VALUES (N'انواع نشریات', N'/publication-types', NULL, @BaseInfoParentId, 5, 1, 0);
                END
                
                -- کاربران
                IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuItem] WHERE [Label] = N'کاربران' AND [ParentId] IS NULL)
                BEGIN
                    DECLARE @UsersParentId INT;
                    INSERT INTO [dbo].[MenuItem] ([Label], [Path], [Icon], [ParentId], [DisplayOrder], [IsActive], [IsSystem])
                    VALUES (N'کاربران', NULL, N'FaUsers', NULL, 5, 1, 0);
                    SET @UsersParentId = SCOPE_IDENTITY();
                    
                    INSERT INTO [dbo].[MenuItem] ([Label], [Path], [Icon], [ParentId], [DisplayOrder], [IsActive], [IsSystem])
                    VALUES (N'مدیریت کاربران', N'/users', NULL, @UsersParentId, 0, 1, 0);
                END
                
                -- پروفایل
                IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuItem] WHERE [Label] = N'پروفایل' AND [Path] = N'/profile')
                BEGIN
                    INSERT INTO [dbo].[MenuItem] ([Label], [Path], [Icon], [ParentId], [DisplayOrder], [IsActive], [IsSystem])
                    VALUES (N'پروفایل', N'/profile', N'FaUser', NULL, 6, 1, 0);
                END
                
                -- گزارش‌ها
                IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuItem] WHERE [Label] = N'گزارش‌ها' AND [ParentId] IS NULL)
                BEGIN
                    DECLARE @ReportsParentId INT;
                    INSERT INTO [dbo].[MenuItem] ([Label], [Path], [Icon], [ParentId], [DisplayOrder], [IsActive], [IsSystem])
                    VALUES (N'گزارش‌ها', NULL, N'FaFileAlt', NULL, 7, 1, 0);
                    SET @ReportsParentId = SCOPE_IDENTITY();
                    
                    INSERT INTO [dbo].[MenuItem] ([Label], [Path], [Icon], [ParentId], [DisplayOrder], [IsActive], [IsSystem])
                    VALUES (N'جستجوی کتاب', N'/books/search', NULL, @ReportsParentId, 0, 1, 0);
                    
                    INSERT INTO [dbo].[MenuItem] ([Label], [Path], [Icon], [ParentId], [DisplayOrder], [IsActive], [IsSystem])
                    VALUES (N'جستجوی نسخه خطی', N'/manuscripts/search', NULL, @ReportsParentId, 1, 1, 0);
                    
                    INSERT INTO [dbo].[MenuItem] ([Label], [Path], [Icon], [ParentId], [DisplayOrder], [IsActive], [IsSystem])
                    VALUES (N'جستجوی نشریه', N'/publications/search', NULL, @ReportsParentId, 2, 1, 0);
                    
                    INSERT INTO [dbo].[MenuItem] ([Label], [Path], [Icon], [ParentId], [DisplayOrder], [IsActive], [IsSystem])
                    VALUES (N'گزارشات آماری کتاب', N'/books/statistics', NULL, @ReportsParentId, 3, 1, 0);
                    
                    INSERT INTO [dbo].[MenuItem] ([Label], [Path], [Icon], [ParentId], [DisplayOrder], [IsActive], [IsSystem])
                    VALUES (N'گزارشات آماری نسخ خطی', N'/manuscripts/statistics', NULL, @ReportsParentId, 4, 1, 0);
                    
                    INSERT INTO [dbo].[MenuItem] ([Label], [Path], [Icon], [ParentId], [DisplayOrder], [IsActive], [IsSystem])
                    VALUES (N'گزارشات آماری نشریه', N'/publications/statistics', NULL, @ReportsParentId, 5, 1, 0);
                END
                
                -- تنظیمات (با ساختار صحیح: تنظیمات منو مستقیماً زیر تنظیمات)
                IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuItem] WHERE [Label] = N'تنظیمات' AND [ParentId] IS NULL)
                BEGIN
                    DECLARE @SettingsParentId INT;
                    INSERT INTO [dbo].[MenuItem] ([Label], [Path], [Icon], [ParentId], [DisplayOrder], [IsActive], [IsSystem])
                    VALUES (N'تنظیمات', NULL, N'FaCog', NULL, 8, 1, 0);
                    SET @SettingsParentId = SCOPE_IDENTITY();
                    
                    -- تنظیمات عمومی
                    INSERT INTO [dbo].[MenuItem] ([Label], [Path], [Icon], [ParentId], [DisplayOrder], [IsActive], [IsSystem])
                    VALUES (N'تنظیمات عمومی', N'/settings', NULL, @SettingsParentId, 0, 1, 0);
                    
                    -- تنظیمات منو (سیستمی) - مستقیماً زیر تنظیمات (نه زیر تنظیمات عمومی)
                    INSERT INTO [dbo].[MenuItem] ([Label], [Path], [Icon], [ParentId], [DisplayOrder], [IsActive], [IsSystem])
                    VALUES (N'تنظیمات منو', N'/menu-settings', NULL, @SettingsParentId, 1, 1, 1);
                END
                ELSE
                BEGIN
                    -- اگر تنظیمات وجود دارد، ساختار را اصلاح کن
                    DECLARE @SettingsParentId2 INT;
                    SELECT @SettingsParentId2 = [Id] FROM [dbo].[MenuItem] WHERE [Label] = N'تنظیمات' AND [ParentId] IS NULL;
                    
                    -- حذف تنظیمات منو با parent اشتباه (اگر زیر تنظیمات عمومی است)
                    DELETE FROM [dbo].[MenuItem] WHERE [Label] = N'تنظیمات منو';
                    
                    -- اضافه کردن تنظیمات عمومی اگر وجود ندارد
                    IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuItem] WHERE [Label] = N'تنظیمات عمومی' AND [ParentId] = @SettingsParentId2)
                    BEGIN
                        INSERT INTO [dbo].[MenuItem] ([Label], [Path], [Icon], [ParentId], [DisplayOrder], [IsActive], [IsSystem])
                        VALUES (N'تنظیمات عمومی', N'/settings', NULL, @SettingsParentId2, 0, 1, 0);
                    END
                    
                    -- اضافه کردن تنظیمات منو مستقیماً زیر تنظیمات
                    IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuItem] WHERE [Label] = N'تنظیمات منو' AND [ParentId] = @SettingsParentId2)
                    BEGIN
                        INSERT INTO [dbo].[MenuItem] ([Label], [Path], [Icon], [ParentId], [DisplayOrder], [IsActive], [IsSystem])
                        VALUES (N'تنظیمات منو', N'/menu-settings', NULL, @SettingsParentId2, 1, 1, 1);
                    END
                    ELSE
                    BEGIN
                        -- به‌روزرسانی parent و displayOrder
                        UPDATE [dbo].[MenuItem] 
                        SET [ParentId] = @SettingsParentId2, [DisplayOrder] = 1, [IsActive] = 1, [IsSystem] = 1, [Path] = N'/menu-settings'
                        WHERE [Label] = N'تنظیمات منو';
                    END
                END
                
                -- مدیریت ادمین
                IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuItem] WHERE [Label] = N'مدیریت ادمین' AND [ParentId] IS NULL)
                BEGIN
                    DECLARE @AdminParentId INT;
                    INSERT INTO [dbo].[MenuItem] ([Label], [Path], [Icon], [ParentId], [DisplayOrder], [IsActive], [IsSystem])
                    VALUES (N'مدیریت ادمین', NULL, N'FaShieldAlt', NULL, 9, 1, 0);
                    SET @AdminParentId = SCOPE_IDENTITY();
                    
                    INSERT INTO [dbo].[MenuItem] ([Label], [Path], [Icon], [ParentId], [DisplayOrder], [IsActive], [IsSystem])
                    VALUES (N'مدیریت نقش‌ها', N'/admin/roles', NULL, @AdminParentId, 0, 1, 0);
                    
                    INSERT INTO [dbo].[MenuItem] ([Label], [Path], [Icon], [ParentId], [DisplayOrder], [IsActive], [IsSystem])
                    VALUES (N'نقش‌دهی به کاربران', N'/admin/user-roles', NULL, @AdminParentId, 1, 1, 0);
                END
            ");
        }

        
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE [dbo].[MenuItem] NOCHECK CONSTRAINT [FK_MenuItem_MenuItem_ParentId];
                DELETE FROM [dbo].[MenuItem] WHERE [IsSystem] = 0;
                ALTER TABLE [dbo].[MenuItem] CHECK CONSTRAINT [FK_MenuItem_MenuItem_ParentId];
            ");
        }
    }
}
