using Domain.Library;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Authentication;

namespace Infrastructure.Library;

internal static class LibrarySeedData
{
    private static readonly PasswordHasher PasswordHasher = new();

    public static void SeedData(ModelBuilder modelBuilder)
    {
        // Hash password for admin user
        string adminPasswordHash = PasswordHasher.Hash("1234");

        modelBuilder.Entity<Domain.Library.User>().HasData(
            new Domain.Library.User
            {
                Id = 1,
                UserName = "admin",
                Password = adminPasswordHash,
                Name = "admin",
                LastName = "admin"
            }
        );

        modelBuilder.Entity<Settings>().HasData(
            new Settings
            {
                Id = 1,
                Name = "BackgroundImageFolder",
                Value = "D:\\Image"
            },
            new Settings
            {
                Id = 2,
                Name = "CurrentBackgroundImageName",
                Value = "library.jpg"
            },
            new Settings
            {
                Id = 3,
                Name = "MainTitle",
                Value = "فرم اصلی"
            }
        );

        // Seed data for Languages
        modelBuilder.Entity<Language>().HasData(
            new Language { Id = 1, Name = "فارسی", Abbreviation = "FA" },
            new Language { Id = 2, Name = "English", Abbreviation = "EN" },
            new Language { Id = 3, Name = "العربية", Abbreviation = "AR" }
        );

        // Seed data for Cities
        modelBuilder.Entity<City>().HasData(
            new City { Id = 1, Name = "تهران" },
            new City { Id = 2, Name = "اصفهان" },
            new City { Id = 3, Name = "شیراز" }
        );

        // Seed data for Subjects
        modelBuilder.Entity<Subject>().HasData(
            new Subject { Id = 1, Title = "ادبیات" },
            new Subject { Id = 2, Title = "تاریخ" },
            new Subject { Id = 3, Title = "فلسفه" },
            new Subject { Id = 4, Title = "علوم" }
        );

        // Seed data for PublicationTypes
        modelBuilder.Entity<PublicationType>().HasData(
            new PublicationType { Id = 1, Title = "مجله" },
            new PublicationType { Id = 2, Title = "روزنامه" },
            new PublicationType { Id = 3, Title = "نشریه" }
        );

        // Seed data for Publishers
        modelBuilder.Entity<Publisher>().HasData(
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
            }
        );

        // Seed data for Gaps
        modelBuilder.Entity<Gap>().HasData(
            new Gap { Id = 1, Title = "گپ 1", State = true, Note = "توضیحات گپ 1" },
            new Gap { Id = 2, Title = "گپ 2", State = false, Note = "توضیحات گپ 2" }
        );

                // Roles
                modelBuilder.Entity<Role>().HasData(
                    new Role { Id = 1, Name = "Admin" },
                    new Role { Id = 2, Name = "User" }
                );

                // UserRoles (assign admin to all roles)
                modelBuilder.Entity<UserRole>().HasData(
                    new UserRole { Id = 1, UserName = "admin", RoleId = 1 }, // Admin role
                    new UserRole { Id = 2, UserName = "admin", RoleId = 2 }  // User role
                );
    }
}

