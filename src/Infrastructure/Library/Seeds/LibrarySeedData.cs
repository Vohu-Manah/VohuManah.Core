using System;
using System.Collections.Immutable;
using Application.Library.Settings;
using Domain.Library;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Library;

internal static class LibrarySeedData
{
    private const string AdminPasswordHash = "9843417E11AFA30B39EB2E974DC2DCB2E53D4B608E665E478A1686C065107225-71815962DF6849A69BE50976ECA3DA44";

    public static void SeedData(ModelBuilder modelBuilder)
    {
        SeedUsers(modelBuilder);
        SeedSettings(modelBuilder);
        SeedLanguages(modelBuilder);
        SeedCities(modelBuilder);
        SeedSubjects(modelBuilder);
        SeedPublicationTypes(modelBuilder);
        SeedPublisher(modelBuilder);
        SeedGaps(modelBuilder);
        SeedRoles(modelBuilder);
        SeedUserRoles(modelBuilder);
        SeedAppInfo(modelBuilder);
        SeedMenuItems(modelBuilder);
        SeedRoleEndpoints(modelBuilder);
    }

    private static void SeedUsers(ModelBuilder mb)
    {
        mb.Entity<Domain.Library.User>().HasData(
            new Domain.Library.User
            {
                Id = 1,
                UserName = "admin",
                Password = AdminPasswordHash,
                Name = "admin",
                LastName = "admin"
            });
    }

    private static void SeedSettings(ModelBuilder mb)
    {
        mb.Entity<Settings>().HasData(
            new Settings { Id = 1, Name = "BackgroundImageFolder", Value = "D:\\Image" },
            new Settings { Id = 2, Name = "CurrentBackgroundImageName", Value = "library.jpg" },
            new Settings { Id = 3, Name = "MainTitle", Value = "فرم اصلی" });
    }

    private static void SeedLanguages(ModelBuilder mb)
    {
        mb.Entity<Language>().HasData(
            new Language { Id = 1, Name = "فارسی", Abbreviation = "FA" },
            new Language { Id = 2, Name = "English", Abbreviation = "EN" },
            new Language { Id = 3, Name = "العربية", Abbreviation = "AR" });
    }

    private static void SeedCities(ModelBuilder mb)
    {
        mb.Entity<City>().HasData(
            new City { Id = 1, Name = "تهران" },
            new City { Id = 2, Name = "اصفهان" },
            new City { Id = 3, Name = "شیراز" });
    }

    private static void SeedSubjects(ModelBuilder mb)
    {
        mb.Entity<Subject>().HasData(
            new Subject { Id = 1, Title = "ادبیات" },
            new Subject { Id = 2, Title = "تاریخ" },
            new Subject { Id = 3, Title = "فلسفه" },
            new Subject { Id = 4, Title = "علوم" });
    }

    private static void SeedPublicationTypes(ModelBuilder mb)
    {
        mb.Entity<PublicationType>().HasData(
            new PublicationType { Id = 1, Title = "مجله" },
            new PublicationType { Id = 2, Title = "روزنامه" },
            new PublicationType { Id = 3, Title = "نشریه" });
    }

    private static void SeedPublisher(ModelBuilder mb)
    {
        mb.Entity<Publisher>().HasData(
            new Publisher
            {
                Id = 1,
                Name = "انتشارات امیرکبیر",
                ManagerName = "مدیر انتشارات",
                PlaceId = 1,
                Address = "تهران، خیابان انقلاب",
                Telephone = "021-12345678",
                Website = "www.amirkabir.com",
                Email = "info@amirkabir.com"
            });
    }

    private static void SeedGaps(ModelBuilder mb)
    {
        mb.Entity<Gap>().HasData(
            new Gap { Id = 1, Title = "گپ 1", State = true, Note = "توضیحات گپ 1" },
            new Gap { Id = 2, Title = "گپ 2", State = false, Note = "توضیحات گپ 2" });
    }

    private static void SeedRoles(ModelBuilder mb)
    {
        mb.Entity<Role>().HasData(
            new Role { Id = 1, Name = "Admin" },
            new Role { Id = 2, Name = "User" },
            new Role { Id = 3, Name = RolePermissionDefaults.Librarian },
            new Role { Id = 4, Name = RolePermissionDefaults.Publisher },
            new Role { Id = 5, Name = RolePermissionDefaults.Author });
    }

    private static void SeedUserRoles(ModelBuilder mb)
    {
        mb.Entity<UserRole>().HasData(
            new UserRole { Id = 1, UserId = 1, UserName = "admin", RoleId = 1 },
            new UserRole { Id = 2, UserId = 1, UserName = "admin", RoleId = 2 },
            new UserRole { Id = 3, UserId = 1, UserName = "admin", RoleId = 3 },
            new UserRole { Id = 4, UserId = 1, UserName = "admin", RoleId = 4 },
            new UserRole { Id = 5, UserId = 1, UserName = "admin", RoleId = 5 });
    }

    private static void SeedAppInfo(ModelBuilder mb)
    {
        var currentYear = DateTime.UtcNow.Year;
        mb.Entity<AppInfo>().HasData(
            new AppInfo
            {
                Id = 1,
                AppName = "وُهُومَنَهْ",
                Author = "Vohu-Manah Team",
                Copyright = $"© {currentYear}  Vohu Manah",
                Description = "سیستم مدیریت کتابخانه",
                License = "GPL-3.0",
                SupportEmail = "support@vohumanah.com",
                Version = "1.0.0",
                Website = "https://vohu-manah.github.io/"
            });
    }

    private static void SeedMenuItems(ModelBuilder mb)
    {
        mb.Entity<MenuItem>().HasData(
            new MenuItem { Id = 1, Label = "داشبورد", Path = "/", Icon = "FaHome", DisplayOrder = 0, IsActive = true, IsSystem = false },
            
            new MenuItem { Id = 2, Label = "کتاب ها", Icon = "FaBook", DisplayOrder = 1, IsActive = true, IsSystem = false },
            new MenuItem { Id = 3, Label = "مدیریت کتاب ها", Path = "/books", ParentId = 2, DisplayOrder = 0, IsActive = true, IsSystem = false },
            new MenuItem { Id = 4, Label = "جستجوی کتاب", Path = "/books/search", ParentId = 2, DisplayOrder = 1, IsActive = true, IsSystem = false },
            new MenuItem { Id = 5, Label = "آمار کتاب ها", Path = "/books/statistics", ParentId = 2, DisplayOrder = 2, IsActive = true, IsSystem = false },
            
            new MenuItem { Id = 6, Label = "نشریات", Icon = "FaNewspaper", DisplayOrder = 2, IsActive = true, IsSystem = false },
            new MenuItem { Id = 7, Label = "مدیریت نشریات", Path = "/publications", ParentId = 6, DisplayOrder = 0, IsActive = true, IsSystem = false },
            new MenuItem { Id = 8, Label = "جستجوی نشریه", Path = "/publications/search", ParentId = 6, DisplayOrder = 1, IsActive = true, IsSystem = false },
            new MenuItem { Id = 9, Label = "آمار نشریات", Path = "/publications/statistics", ParentId = 6, DisplayOrder = 2, IsActive = true, IsSystem = false },
            
            new MenuItem { Id = 10, Label = "نسخ خطی", Icon = "FaScroll", DisplayOrder = 3, IsActive = true, IsSystem = false },
            new MenuItem { Id = 11, Label = "مدیریت نسخ خطی", Path = "/manuscripts", ParentId = 10, DisplayOrder = 0, IsActive = true, IsSystem = false },
            new MenuItem { Id = 12, Label = "جستجوی نسخه خطی", Path = "/manuscripts/search", ParentId = 10, DisplayOrder = 1, IsActive = true, IsSystem = false },
            new MenuItem { Id = 13, Label = "آمار نسخ خطی", Path = "/manuscripts/statistics", ParentId = 10, DisplayOrder = 2, IsActive = true, IsSystem = false },
            
            new MenuItem { Id = 14, Label = "اطلاعات پایه", Icon = "FaTags", DisplayOrder = 4, IsActive = true, IsSystem = false },
            new MenuItem { Id = 15, Label = "شهرها", Path = "/cities", ParentId = 14, DisplayOrder = 0, IsActive = true, IsSystem = false },
            new MenuItem { Id = 16, Label = "زبان ها", Path = "/languages", ParentId = 14, DisplayOrder = 1, IsActive = true, IsSystem = false },
            new MenuItem { Id = 17, Label = "موضوعات", Path = "/subjects", ParentId = 14, DisplayOrder = 2, IsActive = true, IsSystem = false },
            new MenuItem { Id = 18, Label = "ناشران", Path = "/publishers", ParentId = 14, DisplayOrder = 3, IsActive = true, IsSystem = false },
            new MenuItem { Id = 19, Label = "فاصله ها", Path = "/gaps", ParentId = 14, DisplayOrder = 4, IsActive = true, IsSystem = false },
            new MenuItem { Id = 20, Label = "انواع نشریات", Path = "/publication-types", ParentId = 14, DisplayOrder = 5, IsActive = true, IsSystem = false },
            
            new MenuItem { Id = 21, Label = "کاربران", Icon = "FaUsers", DisplayOrder = 5, IsActive = true, IsSystem = false },
            new MenuItem { Id = 22, Label = "مدیریت کاربران", Path = "/users", ParentId = 21, DisplayOrder = 0, IsActive = true, IsSystem = false },
            
            new MenuItem { Id = 23, Label = "پروفایل", Path = "/profile", Icon = "FaUser", DisplayOrder = 6, IsActive = true, IsSystem = false },
            
            new MenuItem { Id = 24, Label = "گزارش ها", Icon = "FaFileAlt", DisplayOrder = 7, IsActive = true, IsSystem = false },
            new MenuItem { Id = 25, Label = "جستجوی کتاب", Path = "/books/search", ParentId = 24, DisplayOrder = 0, IsActive = true, IsSystem = false },
            new MenuItem { Id = 26, Label = "جستجوی نسخه خطی", Path = "/manuscripts/search", ParentId = 24, DisplayOrder = 1, IsActive = true, IsSystem = false },
            new MenuItem { Id = 27, Label = "جستجوی نشریه", Path = "/publications/search", ParentId = 24, DisplayOrder = 2, IsActive = true, IsSystem = false },
            new MenuItem { Id = 28, Label = "گزارشات آماری کتاب", Path = "/books/statistics", ParentId = 24, DisplayOrder = 3, IsActive = true, IsSystem = false },
            new MenuItem { Id = 29, Label = "گزارشات آماری نسخ خطی", Path = "/manuscripts/statistics", ParentId = 24, DisplayOrder = 4, IsActive = true, IsSystem = false },
            new MenuItem { Id = 30, Label = "گزارشات آماری نشریه", Path = "/publications/statistics", ParentId = 24, DisplayOrder = 5, IsActive = true, IsSystem = false },
            
            new MenuItem { Id = 31, Label = "تنظیمات", Icon = "FaCog", DisplayOrder = 8, IsActive = true, IsSystem = false },
            new MenuItem { Id = 32, Label = "تنظیمات عمومی", Path = "/settings", ParentId = 31, DisplayOrder = 0, IsActive = true, IsSystem = false },
            new MenuItem { Id = 33, Label = "تنظیمات منو", Path = "/menu-settings", ParentId = 31, DisplayOrder = 1, IsActive = true, IsSystem = true },
            
            new MenuItem { Id = 34, Label = "مدیریت ادمین", Icon = "FaShieldAlt", DisplayOrder = 9, IsActive = true, IsSystem = false },
            new MenuItem { Id = 35, Label = "مدیریت نقش ها", Path = "/admin/roles", ParentId = 34, DisplayOrder = 0, IsActive = true, IsSystem = false },
            new MenuItem { Id = 36, Label = "نقش دهی به کاربران", Path = "/admin/user-roles", ParentId = 34, DisplayOrder = 1, IsActive = true, IsSystem = false });
    }

    private static void SeedRoleEndpoints(ModelBuilder mb)
    {
        var rows = new List<RoleEndpoint>();
        int nextId = 1;

        foreach (var ep in EndpointCatalog.All.OrderBy(x => x, StringComparer.OrdinalIgnoreCase))
        {
            rows.Add(new RoleEndpoint { Id = nextId++, RoleId = 1, EndpointName = ep });
        }


        AddRoleEndpoints(rows, ref nextId, 2, RolePermissionDefaults.UserEndpoints);
        AddRoleEndpoints(rows, ref nextId, 3, RolePermissionDefaults.LibrarianEndpoints);
        AddRoleEndpoints(rows, ref nextId, 4, RolePermissionDefaults.PublisherEndpoints);
        AddRoleEndpoints(rows, ref nextId, 5, RolePermissionDefaults.AuthorEndpoints);

        mb.Entity<RoleEndpoint>().HasData(rows);
    }

    private static void AddRoleEndpoints(
        List<RoleEndpoint> rows, ref int nextId, int roleId, ImmutableArray<string> endpoints)
    {
        foreach (var ep in endpoints)
        {
            rows.Add(new RoleEndpoint { Id = nextId++, RoleId = roleId, EndpointName = ep });
        }
    }
}
