
using System;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace VohuManah.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "10.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Domain.Library.AppInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AppName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Author")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Copyright")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("Description")
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<DateTime?>("LastUpdateDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("License")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("SupportEmail")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Version")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Website")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.HasKey("Id");

                    b.ToTable("AppInfo", "dbo");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            AppName = "وُهُومَنَهْ",
                            Author = "Vohu-Manah Team",
                            Copyright = "© 2026  Vohu Manah",
                            Description = "سیستم مدیریت کتابخانه",
                            License = "GPL-3.0",
                            SupportEmail = "support@vohumanah.com",
                            Version = "1.0.0",
                            Website = "https://vohu-manah.github.io/"
                        });
                });

            modelBuilder.Entity("Domain.Library.Attachment", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("ContentType")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Description")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<long>("EntityId")
                        .HasColumnType("bigint");

                    b.Property<string>("EntityType")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("FilePath")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<long>("FileSize")
                        .HasColumnType("bigint");

                    b.Property<bool>("IsMain")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<string>("OriginalFileName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("EntityType", "EntityId");

                    b.ToTable("Attachment", "dbo");
                });

            modelBuilder.Entity("Domain.Library.Book", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("Author")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("BookShelfRow")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<string>("Corrector")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Editor")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Isbn")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<int>("LanguageId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("No")
                        .IsRequired()
                        .HasMaxLength(800)
                        .HasColumnType("nvarchar(800)");

                    b.Property<string>("PublishOrder")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<int>("PublishPlaceId")
                        .HasColumnType("int");

                    b.Property<string>("PublishYear")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<int>("PublisherId")
                        .HasColumnType("int");

                    b.Property<int>("SubjectId")
                        .HasColumnType("int");

                    b.Property<string>("Translator")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<int>("VolumeCount")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Author");

                    b.HasIndex("Isbn");

                    b.HasIndex("LanguageId");

                    b.HasIndex("Name");

                    b.HasIndex("No");

                    b.HasIndex("PublishPlaceId");

                    b.HasIndex("PublishYear");

                    b.HasIndex("PublisherId");

                    b.HasIndex("SubjectId");

                    b.ToTable("Book", "dbo");
                });

            modelBuilder.Entity("Domain.Library.City", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("City", "dbo");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "تهران"
                        },
                        new
                        {
                            Id = 2,
                            Name = "اصفهان"
                        },
                        new
                        {
                            Id = 3,
                            Name = "شیراز"
                        });
                });

            modelBuilder.Entity("Domain.Library.Gap", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Note")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<bool>("State")
                        .HasColumnType("bit");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.ToTable("Gap", "dbo");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Note = "توضیحات گپ 1",
                            State = true,
                            Title = "گپ 1"
                        },
                        new
                        {
                            Id = 2,
                            Note = "توضیحات گپ 2",
                            State = false,
                            Title = "گپ 2"
                        });
                });

            modelBuilder.Entity("Domain.Library.Language", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Abbreviation")
                        .IsRequired()
                        .HasMaxLength(5)
                        .HasColumnType("nvarchar(5)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.HasKey("Id");

                    b.ToTable("Language", "dbo");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Abbreviation = "FA",
                            Name = "فارسی"
                        },
                        new
                        {
                            Id = 2,
                            Abbreviation = "EN",
                            Name = "English"
                        },
                        new
                        {
                            Id = 3,
                            Abbreviation = "AR",
                            Name = "العربية"
                        });
                });

            modelBuilder.Entity("Domain.Library.Manuscript", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Author")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<int>("GapId")
                        .HasColumnType("int");

                    b.Property<int>("LanguageId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("PageCount")
                        .HasColumnType("int");

                    b.Property<string>("Size")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("SubjectId")
                        .HasColumnType("int");

                    b.Property<string>("VersionNo")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<int>("WritingPlaceId")
                        .HasColumnType("int");

                    b.Property<string>("Year")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.HasKey("Id");

                    b.HasIndex("Author");

                    b.HasIndex("GapId");

                    b.HasIndex("LanguageId");

                    b.HasIndex("Name");

                    b.HasIndex("SubjectId");

                    b.HasIndex("VersionNo");

                    b.HasIndex("WritingPlaceId");

                    b.HasIndex("Year");

                    b.ToTable("Manuscript", "dbo");
                });

            modelBuilder.Entity("Domain.Library.MenuItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("DisplayOrder")
                        .HasColumnType("int");

                    b.Property<string>("Icon")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsSystem")
                        .HasColumnType("bit");

                    b.Property<string>("Label")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<int?>("ParentId")
                        .HasColumnType("int");

                    b.Property<string>("Path")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.HasKey("Id");

                    b.HasIndex("DisplayOrder");

                    b.HasIndex("ParentId", "DisplayOrder");

                    b.ToTable("MenuItem", "dbo");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            DisplayOrder = 0,
                            Icon = "FaHome",
                            IsActive = true,
                            IsSystem = false,
                            Label = "داشبورد",
                            Path = "/"
                        },
                        new
                        {
                            Id = 2,
                            DisplayOrder = 1,
                            Icon = "FaBook",
                            IsActive = true,
                            IsSystem = false,
                            Label = "کتاب ها"
                        },
                        new
                        {
                            Id = 3,
                            DisplayOrder = 0,
                            IsActive = true,
                            IsSystem = false,
                            Label = "مدیریت کتاب ها",
                            ParentId = 2,
                            Path = "/books"
                        },
                        new
                        {
                            Id = 4,
                            DisplayOrder = 1,
                            IsActive = true,
                            IsSystem = false,
                            Label = "جستجوی کتاب",
                            ParentId = 2,
                            Path = "/books/search"
                        },
                        new
                        {
                            Id = 5,
                            DisplayOrder = 2,
                            IsActive = true,
                            IsSystem = false,
                            Label = "آمار کتاب ها",
                            ParentId = 2,
                            Path = "/books/statistics"
                        },
                        new
                        {
                            Id = 6,
                            DisplayOrder = 2,
                            Icon = "FaNewspaper",
                            IsActive = true,
                            IsSystem = false,
                            Label = "نشریات"
                        },
                        new
                        {
                            Id = 7,
                            DisplayOrder = 0,
                            IsActive = true,
                            IsSystem = false,
                            Label = "مدیریت نشریات",
                            ParentId = 6,
                            Path = "/publications"
                        },
                        new
                        {
                            Id = 8,
                            DisplayOrder = 1,
                            IsActive = true,
                            IsSystem = false,
                            Label = "جستجوی نشریه",
                            ParentId = 6,
                            Path = "/publications/search"
                        },
                        new
                        {
                            Id = 9,
                            DisplayOrder = 2,
                            IsActive = true,
                            IsSystem = false,
                            Label = "آمار نشریات",
                            ParentId = 6,
                            Path = "/publications/statistics"
                        },
                        new
                        {
                            Id = 10,
                            DisplayOrder = 3,
                            Icon = "FaScroll",
                            IsActive = true,
                            IsSystem = false,
                            Label = "نسخ خطی"
                        },
                        new
                        {
                            Id = 11,
                            DisplayOrder = 0,
                            IsActive = true,
                            IsSystem = false,
                            Label = "مدیریت نسخ خطی",
                            ParentId = 10,
                            Path = "/manuscripts"
                        },
                        new
                        {
                            Id = 12,
                            DisplayOrder = 1,
                            IsActive = true,
                            IsSystem = false,
                            Label = "جستجوی نسخه خطی",
                            ParentId = 10,
                            Path = "/manuscripts/search"
                        },
                        new
                        {
                            Id = 13,
                            DisplayOrder = 2,
                            IsActive = true,
                            IsSystem = false,
                            Label = "آمار نسخ خطی",
                            ParentId = 10,
                            Path = "/manuscripts/statistics"
                        },
                        new
                        {
                            Id = 14,
                            DisplayOrder = 4,
                            Icon = "FaTags",
                            IsActive = true,
                            IsSystem = false,
                            Label = "اطلاعات پایه"
                        },
                        new
                        {
                            Id = 15,
                            DisplayOrder = 0,
                            IsActive = true,
                            IsSystem = false,
                            Label = "شهرها",
                            ParentId = 14,
                            Path = "/cities"
                        },
                        new
                        {
                            Id = 16,
                            DisplayOrder = 1,
                            IsActive = true,
                            IsSystem = false,
                            Label = "زبان ها",
                            ParentId = 14,
                            Path = "/languages"
                        },
                        new
                        {
                            Id = 17,
                            DisplayOrder = 2,
                            IsActive = true,
                            IsSystem = false,
                            Label = "موضوعات",
                            ParentId = 14,
                            Path = "/subjects"
                        },
                        new
                        {
                            Id = 18,
                            DisplayOrder = 3,
                            IsActive = true,
                            IsSystem = false,
                            Label = "ناشران",
                            ParentId = 14,
                            Path = "/publishers"
                        },
                        new
                        {
                            Id = 19,
                            DisplayOrder = 4,
                            IsActive = true,
                            IsSystem = false,
                            Label = "فاصله ها",
                            ParentId = 14,
                            Path = "/gaps"
                        },
                        new
                        {
                            Id = 20,
                            DisplayOrder = 5,
                            IsActive = true,
                            IsSystem = false,
                            Label = "انواع نشریات",
                            ParentId = 14,
                            Path = "/publication-types"
                        },
                        new
                        {
                            Id = 21,
                            DisplayOrder = 5,
                            Icon = "FaUsers",
                            IsActive = true,
                            IsSystem = false,
                            Label = "کاربران"
                        },
                        new
                        {
                            Id = 22,
                            DisplayOrder = 0,
                            IsActive = true,
                            IsSystem = false,
                            Label = "مدیریت کاربران",
                            ParentId = 21,
                            Path = "/users"
                        },
                        new
                        {
                            Id = 23,
                            DisplayOrder = 6,
                            Icon = "FaUser",
                            IsActive = true,
                            IsSystem = false,
                            Label = "پروفایل",
                            Path = "/profile"
                        },
                        new
                        {
                            Id = 24,
                            DisplayOrder = 7,
                            Icon = "FaFileAlt",
                            IsActive = true,
                            IsSystem = false,
                            Label = "گزارش ها"
                        },
                        new
                        {
                            Id = 25,
                            DisplayOrder = 0,
                            IsActive = true,
                            IsSystem = false,
                            Label = "جستجوی کتاب",
                            ParentId = 24,
                            Path = "/books/search"
                        },
                        new
                        {
                            Id = 26,
                            DisplayOrder = 1,
                            IsActive = true,
                            IsSystem = false,
                            Label = "جستجوی نسخه خطی",
                            ParentId = 24,
                            Path = "/manuscripts/search"
                        },
                        new
                        {
                            Id = 27,
                            DisplayOrder = 2,
                            IsActive = true,
                            IsSystem = false,
                            Label = "جستجوی نشریه",
                            ParentId = 24,
                            Path = "/publications/search"
                        },
                        new
                        {
                            Id = 28,
                            DisplayOrder = 3,
                            IsActive = true,
                            IsSystem = false,
                            Label = "گزارشات آماری کتاب",
                            ParentId = 24,
                            Path = "/books/statistics"
                        },
                        new
                        {
                            Id = 29,
                            DisplayOrder = 4,
                            IsActive = true,
                            IsSystem = false,
                            Label = "گزارشات آماری نسخ خطی",
                            ParentId = 24,
                            Path = "/manuscripts/statistics"
                        },
                        new
                        {
                            Id = 30,
                            DisplayOrder = 5,
                            IsActive = true,
                            IsSystem = false,
                            Label = "گزارشات آماری نشریه",
                            ParentId = 24,
                            Path = "/publications/statistics"
                        },
                        new
                        {
                            Id = 31,
                            DisplayOrder = 8,
                            Icon = "FaCog",
                            IsActive = true,
                            IsSystem = false,
                            Label = "تنظیمات"
                        },
                        new
                        {
                            Id = 32,
                            DisplayOrder = 0,
                            IsActive = true,
                            IsSystem = false,
                            Label = "تنظیمات عمومی",
                            ParentId = 31,
                            Path = "/settings"
                        },
                        new
                        {
                            Id = 33,
                            DisplayOrder = 1,
                            IsActive = true,
                            IsSystem = true,
                            Label = "تنظیمات منو",
                            ParentId = 31,
                            Path = "/menu-settings"
                        },
                        new
                        {
                            Id = 34,
                            DisplayOrder = 9,
                            Icon = "FaShieldAlt",
                            IsActive = true,
                            IsSystem = false,
                            Label = "مدیریت ادمین"
                        },
                        new
                        {
                            Id = 35,
                            DisplayOrder = 0,
                            IsActive = true,
                            IsSystem = false,
                            Label = "مدیریت نقش ها",
                            ParentId = 34,
                            Path = "/admin/roles"
                        },
                        new
                        {
                            Id = 36,
                            DisplayOrder = 1,
                            IsActive = true,
                            IsSystem = false,
                            Label = "نقش دهی به کاربران",
                            ParentId = 34,
                            Path = "/admin/user-roles"
                        });
                });

            modelBuilder.Entity("Domain.Library.Publication", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Concessionaire")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Editor")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("JournalNo")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<int>("LanguageId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("No")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("Period")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("PublishDate")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("PublishPlaceId")
                        .HasColumnType("int");

                    b.Property<string>("ResponsibleDirector")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("SubjectId")
                        .HasColumnType("int");

                    b.Property<int>("TypeId")
                        .HasColumnType("int");

                    b.Property<string>("Year")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.HasKey("Id");

                    b.HasIndex("JournalNo");

                    b.HasIndex("LanguageId");

                    b.HasIndex("Name");

                    b.HasIndex("No");

                    b.HasIndex("PublishPlaceId");

                    b.HasIndex("SubjectId");

                    b.HasIndex("TypeId");

                    b.HasIndex("Year");

                    b.ToTable("Publication", "dbo");
                });

            modelBuilder.Entity("Domain.Library.PublicationType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("PublicationType", "dbo");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Title = "مجله"
                        },
                        new
                        {
                            Id = 2,
                            Title = "روزنامه"
                        },
                        new
                        {
                            Id = 3,
                            Title = "نشریه"
                        });
                });

            modelBuilder.Entity("Domain.Library.Publisher", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("ManagerName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("PlaceId")
                        .HasColumnType("int");

                    b.Property<string>("Telephone")
                        .IsRequired()
                        .HasMaxLength(15)
                        .HasColumnType("nvarchar(15)");

                    b.Property<string>("Website")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.ToTable("Publisher", "dbo");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Address = "تهران، خیابان انقلاب",
                            Email = "info@amirkabir.com",
                            ManagerName = "مدیر انتشارات",
                            Name = "انتشارات امیرکبیر",
                            PlaceId = 1,
                            Telephone = "021-12345678",
                            Website = "www.amirkabir.com"
                        });
                });

            modelBuilder.Entity("Domain.Library.RefreshToken", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("ExpiresAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("RevokedAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("RefreshTokens", "dbo");
                });

            modelBuilder.Entity("Domain.Library.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("Role", "dbo");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Admin"
                        },
                        new
                        {
                            Id = 2,
                            Name = "User"
                        },
                        new
                        {
                            Id = 3,
                            Name = "Librarian"
                        },
                        new
                        {
                            Id = 4,
                            Name = "Publisher"
                        },
                        new
                        {
                            Id = 5,
                            Name = "Author"
                        });
                });

            modelBuilder.Entity("Domain.Library.RoleEndpoint", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("EndpointName")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("RoleId", "EndpointName")
                        .IsUnique();

                    b.ToTable("RolePermission", "dbo");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            EndpointName = "Library.Attachments.Delete",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 2,
                            EndpointName = "Library.Attachments.GetByEntity",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 3,
                            EndpointName = "Library.Attachments.Update",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 4,
                            EndpointName = "Library.Attachments.Upload",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 5,
                            EndpointName = "Library.Backup.Create",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 6,
                            EndpointName = "Library.Backup.Download",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 7,
                            EndpointName = "Library.Backup.GetInfo",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 8,
                            EndpointName = "Library.Backup.Restore",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 9,
                            EndpointName = "Library.Books.Create",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 10,
                            EndpointName = "Library.Books.Delete",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 11,
                            EndpointName = "Library.Books.GetAll",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 12,
                            EndpointName = "Library.Books.GetAllEntities",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 13,
                            EndpointName = "Library.Books.GetById",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 14,
                            EndpointName = "Library.Books.GetCount",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 15,
                            EndpointName = "Library.Books.GetCountByLanguage",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 16,
                            EndpointName = "Library.Books.GetCountByPublisher",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 17,
                            EndpointName = "Library.Books.GetCountByPublishPlace",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 18,
                            EndpointName = "Library.Books.GetCountBySubject",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 19,
                            EndpointName = "Library.Books.GetCountByYear",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 20,
                            EndpointName = "Library.Books.GetList",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 21,
                            EndpointName = "Library.Books.GetProfile",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 22,
                            EndpointName = "Library.Books.GetStatistics",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 23,
                            EndpointName = "Library.Books.GetVolumeCount",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 24,
                            EndpointName = "Library.Books.Search",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 25,
                            EndpointName = "Library.Books.Update",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 26,
                            EndpointName = "Library.Cities.Create",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 27,
                            EndpointName = "Library.Cities.Delete",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 28,
                            EndpointName = "Library.Cities.GetAll",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 29,
                            EndpointName = "Library.Cities.GetById",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 30,
                            EndpointName = "Library.Cities.GetList",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 31,
                            EndpointName = "Library.Cities.GetNames",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 32,
                            EndpointName = "Library.Cities.Update",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 33,
                            EndpointName = "Library.Gaps.Create",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 34,
                            EndpointName = "Library.Gaps.Delete",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 35,
                            EndpointName = "Library.Gaps.GetAll",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 36,
                            EndpointName = "Library.Gaps.GetById",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 37,
                            EndpointName = "Library.Gaps.GetList",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 38,
                            EndpointName = "Library.Gaps.GetNames",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 39,
                            EndpointName = "Library.Gaps.Update",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 40,
                            EndpointName = "Library.Languages.Create",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 41,
                            EndpointName = "Library.Languages.Delete",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 42,
                            EndpointName = "Library.Languages.GetAll",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 43,
                            EndpointName = "Library.Languages.GetById",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 44,
                            EndpointName = "Library.Languages.GetList",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 45,
                            EndpointName = "Library.Languages.GetNames",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 46,
                            EndpointName = "Library.Languages.Update",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 47,
                            EndpointName = "Library.Manuscripts.Create",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 48,
                            EndpointName = "Library.Manuscripts.Delete",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 49,
                            EndpointName = "Library.Manuscripts.GetAll",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 50,
                            EndpointName = "Library.Manuscripts.GetAllEntities",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 51,
                            EndpointName = "Library.Manuscripts.GetById",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 52,
                            EndpointName = "Library.Manuscripts.GetCount",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 53,
                            EndpointName = "Library.Manuscripts.GetCountByGap",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 54,
                            EndpointName = "Library.Manuscripts.GetCountByLanguage",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 55,
                            EndpointName = "Library.Manuscripts.GetCountBySubject",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 56,
                            EndpointName = "Library.Manuscripts.GetCountByWritingPlace",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 57,
                            EndpointName = "Library.Manuscripts.GetCountByYear",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 58,
                            EndpointName = "Library.Manuscripts.GetList",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 59,
                            EndpointName = "Library.Manuscripts.GetStatistics",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 60,
                            EndpointName = "Library.Manuscripts.Search",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 61,
                            EndpointName = "Library.Manuscripts.Update",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 62,
                            EndpointName = "Library.Menu.Create",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 63,
                            EndpointName = "Library.Menu.Delete",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 64,
                            EndpointName = "Library.Menu.GetAll",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 65,
                            EndpointName = "Library.Menu.Reorder",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 66,
                            EndpointName = "Library.Menu.ResetToDefault",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 67,
                            EndpointName = "Library.Menu.Update",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 68,
                            EndpointName = "Library.Publications.Create",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 69,
                            EndpointName = "Library.Publications.Delete",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 70,
                            EndpointName = "Library.Publications.GetAll",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 71,
                            EndpointName = "Library.Publications.GetAllEntities",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 72,
                            EndpointName = "Library.Publications.GetById",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 73,
                            EndpointName = "Library.Publications.GetCount",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 74,
                            EndpointName = "Library.Publications.GetCountByLanguage",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 75,
                            EndpointName = "Library.Publications.GetCountByPublishDate",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 76,
                            EndpointName = "Library.Publications.GetCountByPublishPlace",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 77,
                            EndpointName = "Library.Publications.GetCountBySubject",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 78,
                            EndpointName = "Library.Publications.GetCountByType",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 79,
                            EndpointName = "Library.Publications.GetList",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 80,
                            EndpointName = "Library.Publications.GetStatistics",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 81,
                            EndpointName = "Library.Publications.Search",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 82,
                            EndpointName = "Library.Publications.Update",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 83,
                            EndpointName = "Library.PublicationTypes.Create",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 84,
                            EndpointName = "Library.PublicationTypes.Delete",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 85,
                            EndpointName = "Library.PublicationTypes.GetAll",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 86,
                            EndpointName = "Library.PublicationTypes.GetById",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 87,
                            EndpointName = "Library.PublicationTypes.GetList",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 88,
                            EndpointName = "Library.PublicationTypes.GetNames",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 89,
                            EndpointName = "Library.PublicationTypes.Update",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 90,
                            EndpointName = "Library.Publishers.Create",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 91,
                            EndpointName = "Library.Publishers.Delete",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 92,
                            EndpointName = "Library.Publishers.GetAll",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 93,
                            EndpointName = "Library.Publishers.GetById",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 94,
                            EndpointName = "Library.Publishers.GetList",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 95,
                            EndpointName = "Library.Publishers.GetNames",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 96,
                            EndpointName = "Library.Publishers.Update",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 97,
                            EndpointName = "Library.Settings.AssignRoleToUser",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 98,
                            EndpointName = "Library.Settings.CreateRole",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 99,
                            EndpointName = "Library.Settings.GetAll",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 100,
                            EndpointName = "Library.Settings.GetAllEndpoints",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 101,
                            EndpointName = "Library.Settings.GetAllEntities",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 102,
                            EndpointName = "Library.Settings.GetAllRoles",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 103,
                            EndpointName = "Library.Settings.GetAppInfo",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 104,
                            EndpointName = "Library.Settings.GetBackground",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 105,
                            EndpointName = "Library.Settings.GetById",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 106,
                            EndpointName = "Library.Settings.GetByName",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 107,
                            EndpointName = "Library.Settings.GetMainTitle",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 108,
                            EndpointName = "Library.Settings.GetRolePermissions",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 109,
                            EndpointName = "Library.Settings.GetSystemInfo",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 110,
                            EndpointName = "Library.Settings.GetUserRoles",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 111,
                            EndpointName = "Library.Settings.GetUserSettings",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 112,
                            EndpointName = "Library.Settings.RemoveRoleFromUser",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 113,
                            EndpointName = "Library.Settings.Update",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 114,
                            EndpointName = "Library.Settings.UpdateRoleEndpoints",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 115,
                            EndpointName = "Library.Settings.UpdateUserSettings",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 116,
                            EndpointName = "Library.Subjects.Create",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 117,
                            EndpointName = "Library.Subjects.Delete",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 118,
                            EndpointName = "Library.Subjects.GetAll",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 119,
                            EndpointName = "Library.Subjects.GetById",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 120,
                            EndpointName = "Library.Subjects.GetList",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 121,
                            EndpointName = "Library.Subjects.GetNames",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 122,
                            EndpointName = "Library.Subjects.Update",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 123,
                            EndpointName = "Library.Users.ChangePassword",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 124,
                            EndpointName = "Library.Users.Create",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 125,
                            EndpointName = "Library.Users.Delete",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 126,
                            EndpointName = "Library.Users.GetAll",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 127,
                            EndpointName = "Library.Users.GetAllEntities",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 128,
                            EndpointName = "Library.Users.GetByUserName",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 129,
                            EndpointName = "Library.Users.GetFullName",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 130,
                            EndpointName = "Library.Users.GetList",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 131,
                            EndpointName = "Library.Users.Revoke",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 132,
                            EndpointName = "Library.Users.Update",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 133,
                            EndpointName = "Library.Users.UpdateProfile",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 134,
                            EndpointName = "Library.Visits.GetStats",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 135,
                            EndpointName = "Reports.BookSearchList",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 136,
                            EndpointName = "Reports.BookStatistics",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 137,
                            EndpointName = "Reports.Export",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 138,
                            EndpointName = "Reports.ManuscriptSearchList",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 139,
                            EndpointName = "Reports.ManuscriptStatistics",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 140,
                            EndpointName = "Reports.PublicationSearchList",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 141,
                            EndpointName = "Reports.PublicationStatistics",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 142,
                            EndpointName = "Reports.View",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 143,
                            EndpointName = "Library.Settings.GetUserSettings",
                            RoleId = 2
                        },
                        new
                        {
                            Id = 144,
                            EndpointName = "Library.Settings.UpdateUserSettings",
                            RoleId = 2
                        },
                        new
                        {
                            Id = 145,
                            EndpointName = "Library.Settings.GetAppInfo",
                            RoleId = 2
                        },
                        new
                        {
                            Id = 146,
                            EndpointName = "Library.Users.ChangePassword",
                            RoleId = 2
                        },
                        new
                        {
                            Id = 147,
                            EndpointName = "Library.Users.UpdateProfile",
                            RoleId = 2
                        },
                        new
                        {
                            Id = 148,
                            EndpointName = "Library.Menu.GetAll",
                            RoleId = 2
                        },
                        new
                        {
                            Id = 149,
                            EndpointName = "Library.Visits.GetStats",
                            RoleId = 2
                        },
                        new
                        {
                            Id = 150,
                            EndpointName = "Library.Books.GetAll",
                            RoleId = 2
                        },
                        new
                        {
                            Id = 151,
                            EndpointName = "Library.Books.GetAllEntities",
                            RoleId = 2
                        },
                        new
                        {
                            Id = 152,
                            EndpointName = "Library.Books.GetById",
                            RoleId = 2
                        },
                        new
                        {
                            Id = 153,
                            EndpointName = "Library.Books.GetProfile",
                            RoleId = 2
                        },
                        new
                        {
                            Id = 154,
                            EndpointName = "Library.Books.GetCount",
                            RoleId = 2
                        },
                        new
                        {
                            Id = 155,
                            EndpointName = "Library.Books.GetCountByLanguage",
                            RoleId = 2
                        },
                        new
                        {
                            Id = 156,
                            EndpointName = "Library.Books.GetCountByPublisher",
                            RoleId = 2
                        },
                        new
                        {
                            Id = 157,
                            EndpointName = "Library.Books.GetCountByPublishPlace",
                            RoleId = 2
                        },
                        new
                        {
                            Id = 158,
                            EndpointName = "Library.Books.GetCountBySubject",
                            RoleId = 2
                        },
                        new
                        {
                            Id = 159,
                            EndpointName = "Library.Books.GetCountByYear",
                            RoleId = 2
                        },
                        new
                        {
                            Id = 160,
                            EndpointName = "Library.Books.GetList",
                            RoleId = 2
                        },
                        new
                        {
                            Id = 161,
                            EndpointName = "Library.Books.GetStatistics",
                            RoleId = 2
                        },
                        new
                        {
                            Id = 162,
                            EndpointName = "Library.Books.GetVolumeCount",
                            RoleId = 2
                        },
                        new
                        {
                            Id = 163,
                            EndpointName = "Library.Books.Search",
                            RoleId = 2
                        },
                        new
                        {
                            Id = 164,
                            EndpointName = "Library.Manuscripts.GetAll",
                            RoleId = 2
                        },
                        new
                        {
                            Id = 165,
                            EndpointName = "Library.Manuscripts.GetAllEntities",
                            RoleId = 2
                        },
                        new
                        {
                            Id = 166,
                            EndpointName = "Library.Manuscripts.GetById",
                            RoleId = 2
                        },
                        new
                        {
                            Id = 167,
                            EndpointName = "Library.Manuscripts.GetCount",
                            RoleId = 2
                        },
                        new
                        {
                            Id = 168,
                            EndpointName = "Library.Manuscripts.GetCountByGap",
                            RoleId = 2
                        },
                        new
                        {
                            Id = 169,
                            EndpointName = "Library.Manuscripts.GetCountByLanguage",
                            RoleId = 2
                        },
                        new
                        {
                            Id = 170,
                            EndpointName = "Library.Manuscripts.GetCountBySubject",
                            RoleId = 2
                        },
                        new
                        {
                            Id = 171,
                            EndpointName = "Library.Manuscripts.GetCountByWritingPlace",
                            RoleId = 2
                        },
                        new
                        {
                            Id = 172,
                            EndpointName = "Library.Manuscripts.GetCountByYear",
                            RoleId = 2
                        },
                        new
                        {
                            Id = 173,
                            EndpointName = "Library.Manuscripts.GetList",
                            RoleId = 2
                        },
                        new
                        {
                            Id = 174,
                            EndpointName = "Library.Manuscripts.GetStatistics",
                            RoleId = 2
                        },
                        new
                        {
                            Id = 175,
                            EndpointName = "Library.Manuscripts.Search",
                            RoleId = 2
                        },
                        new
                        {
                            Id = 176,
                            EndpointName = "Library.Publications.GetAll",
                            RoleId = 2
                        },
                        new
                        {
                            Id = 177,
                            EndpointName = "Library.Publications.GetAllEntities",
                            RoleId = 2
                        },
                        new
                        {
                            Id = 178,
                            EndpointName = "Library.Publications.GetById",
                            RoleId = 2
                        },
                        new
                        {
                            Id = 179,
                            EndpointName = "Library.Publications.GetCount",
                            RoleId = 2
                        },
                        new
                        {
                            Id = 180,
                            EndpointName = "Library.Publications.GetCountByLanguage",
                            RoleId = 2
                        },
                        new
                        {
                            Id = 181,
                            EndpointName = "Library.Publications.GetCountByPublishDate",
                            RoleId = 2
                        },
                        new
                        {
                            Id = 182,
                            EndpointName = "Library.Publications.GetCountByPublishPlace",
                            RoleId = 2
                        },
                        new
                        {
                            Id = 183,
                            EndpointName = "Library.Publications.GetCountBySubject",
                            RoleId = 2
                        },
                        new
                        {
                            Id = 184,
                            EndpointName = "Library.Publications.GetCountByType",
                            RoleId = 2
                        },
                        new
                        {
                            Id = 185,
                            EndpointName = "Library.Publications.GetList",
                            RoleId = 2
                        },
                        new
                        {
                            Id = 186,
                            EndpointName = "Library.Publications.GetStatistics",
                            RoleId = 2
                        },
                        new
                        {
                            Id = 187,
                            EndpointName = "Library.Publications.Search",
                            RoleId = 2
                        },
                        new
                        {
                            Id = 188,
                            EndpointName = "Reports.BookSearchList",
                            RoleId = 2
                        },
                        new
                        {
                            Id = 189,
                            EndpointName = "Reports.PublicationSearchList",
                            RoleId = 2
                        },
                        new
                        {
                            Id = 190,
                            EndpointName = "Reports.ManuscriptSearchList",
                            RoleId = 2
                        },
                        new
                        {
                            Id = 191,
                            EndpointName = "Reports.ManuscriptStatistics",
                            RoleId = 2
                        },
                        new
                        {
                            Id = 192,
                            EndpointName = "Reports.BookStatistics",
                            RoleId = 2
                        },
                        new
                        {
                            Id = 193,
                            EndpointName = "Reports.PublicationStatistics",
                            RoleId = 2
                        },
                        new
                        {
                            Id = 194,
                            EndpointName = "Reports.Export",
                            RoleId = 2
                        },
                        new
                        {
                            Id = 195,
                            EndpointName = "Reports.View",
                            RoleId = 2
                        },
                        new
                        {
                            Id = 196,
                            EndpointName = "Library.Attachments.GetByEntity",
                            RoleId = 2
                        },
                        new
                        {
                            Id = 197,
                            EndpointName = "Library.Settings.GetUserSettings",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 198,
                            EndpointName = "Library.Settings.UpdateUserSettings",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 199,
                            EndpointName = "Library.Settings.GetAppInfo",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 200,
                            EndpointName = "Library.Users.ChangePassword",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 201,
                            EndpointName = "Library.Users.UpdateProfile",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 202,
                            EndpointName = "Library.Menu.GetAll",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 203,
                            EndpointName = "Library.Visits.GetStats",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 204,
                            EndpointName = "Library.Books.Create",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 205,
                            EndpointName = "Library.Books.Delete",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 206,
                            EndpointName = "Library.Books.GetAll",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 207,
                            EndpointName = "Library.Books.GetAllEntities",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 208,
                            EndpointName = "Library.Books.GetById",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 209,
                            EndpointName = "Library.Books.GetProfile",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 210,
                            EndpointName = "Library.Books.GetCount",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 211,
                            EndpointName = "Library.Books.GetCountByLanguage",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 212,
                            EndpointName = "Library.Books.GetCountByPublisher",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 213,
                            EndpointName = "Library.Books.GetCountByPublishPlace",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 214,
                            EndpointName = "Library.Books.GetCountBySubject",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 215,
                            EndpointName = "Library.Books.GetCountByYear",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 216,
                            EndpointName = "Library.Books.GetList",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 217,
                            EndpointName = "Library.Books.GetStatistics",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 218,
                            EndpointName = "Library.Books.GetVolumeCount",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 219,
                            EndpointName = "Library.Books.Search",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 220,
                            EndpointName = "Library.Books.Update",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 221,
                            EndpointName = "Library.Manuscripts.Create",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 222,
                            EndpointName = "Library.Manuscripts.Delete",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 223,
                            EndpointName = "Library.Manuscripts.GetAll",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 224,
                            EndpointName = "Library.Manuscripts.GetAllEntities",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 225,
                            EndpointName = "Library.Manuscripts.GetById",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 226,
                            EndpointName = "Library.Manuscripts.GetCount",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 227,
                            EndpointName = "Library.Manuscripts.GetCountByGap",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 228,
                            EndpointName = "Library.Manuscripts.GetCountByLanguage",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 229,
                            EndpointName = "Library.Manuscripts.GetCountBySubject",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 230,
                            EndpointName = "Library.Manuscripts.GetCountByWritingPlace",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 231,
                            EndpointName = "Library.Manuscripts.GetCountByYear",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 232,
                            EndpointName = "Library.Manuscripts.GetList",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 233,
                            EndpointName = "Library.Manuscripts.GetStatistics",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 234,
                            EndpointName = "Library.Manuscripts.Search",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 235,
                            EndpointName = "Library.Manuscripts.Update",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 236,
                            EndpointName = "Library.Publications.Create",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 237,
                            EndpointName = "Library.Publications.Delete",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 238,
                            EndpointName = "Library.Publications.GetAll",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 239,
                            EndpointName = "Library.Publications.GetAllEntities",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 240,
                            EndpointName = "Library.Publications.GetById",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 241,
                            EndpointName = "Library.Publications.GetCount",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 242,
                            EndpointName = "Library.Publications.GetCountByLanguage",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 243,
                            EndpointName = "Library.Publications.GetCountByPublishDate",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 244,
                            EndpointName = "Library.Publications.GetCountByPublishPlace",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 245,
                            EndpointName = "Library.Publications.GetCountBySubject",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 246,
                            EndpointName = "Library.Publications.GetCountByType",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 247,
                            EndpointName = "Library.Publications.GetList",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 248,
                            EndpointName = "Library.Publications.GetStatistics",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 249,
                            EndpointName = "Library.Publications.Search",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 250,
                            EndpointName = "Library.Publications.Update",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 251,
                            EndpointName = "Library.PublicationTypes.Create",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 252,
                            EndpointName = "Library.PublicationTypes.Delete",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 253,
                            EndpointName = "Library.PublicationTypes.GetAll",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 254,
                            EndpointName = "Library.PublicationTypes.GetById",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 255,
                            EndpointName = "Library.PublicationTypes.GetList",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 256,
                            EndpointName = "Library.PublicationTypes.GetNames",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 257,
                            EndpointName = "Library.PublicationTypes.Update",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 258,
                            EndpointName = "Library.Publishers.Create",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 259,
                            EndpointName = "Library.Publishers.Delete",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 260,
                            EndpointName = "Library.Publishers.GetAll",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 261,
                            EndpointName = "Library.Publishers.GetById",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 262,
                            EndpointName = "Library.Publishers.GetList",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 263,
                            EndpointName = "Library.Publishers.GetNames",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 264,
                            EndpointName = "Library.Publishers.Update",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 265,
                            EndpointName = "Library.Languages.Create",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 266,
                            EndpointName = "Library.Languages.Delete",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 267,
                            EndpointName = "Library.Languages.GetAll",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 268,
                            EndpointName = "Library.Languages.GetById",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 269,
                            EndpointName = "Library.Languages.GetList",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 270,
                            EndpointName = "Library.Languages.GetNames",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 271,
                            EndpointName = "Library.Languages.Update",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 272,
                            EndpointName = "Library.Cities.Create",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 273,
                            EndpointName = "Library.Cities.Delete",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 274,
                            EndpointName = "Library.Cities.GetAll",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 275,
                            EndpointName = "Library.Cities.GetById",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 276,
                            EndpointName = "Library.Cities.GetList",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 277,
                            EndpointName = "Library.Cities.GetNames",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 278,
                            EndpointName = "Library.Cities.Update",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 279,
                            EndpointName = "Library.Subjects.Create",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 280,
                            EndpointName = "Library.Subjects.Delete",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 281,
                            EndpointName = "Library.Subjects.GetAll",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 282,
                            EndpointName = "Library.Subjects.GetById",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 283,
                            EndpointName = "Library.Subjects.GetList",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 284,
                            EndpointName = "Library.Subjects.GetNames",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 285,
                            EndpointName = "Library.Subjects.Update",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 286,
                            EndpointName = "Library.Gaps.Create",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 287,
                            EndpointName = "Library.Gaps.Delete",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 288,
                            EndpointName = "Library.Gaps.GetAll",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 289,
                            EndpointName = "Library.Gaps.GetById",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 290,
                            EndpointName = "Library.Gaps.GetList",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 291,
                            EndpointName = "Library.Gaps.GetNames",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 292,
                            EndpointName = "Library.Gaps.Update",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 293,
                            EndpointName = "Library.Attachments.Upload",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 294,
                            EndpointName = "Library.Attachments.Delete",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 295,
                            EndpointName = "Library.Attachments.GetByEntity",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 296,
                            EndpointName = "Library.Attachments.Update",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 297,
                            EndpointName = "Reports.BookSearchList",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 298,
                            EndpointName = "Reports.PublicationSearchList",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 299,
                            EndpointName = "Reports.ManuscriptSearchList",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 300,
                            EndpointName = "Reports.ManuscriptStatistics",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 301,
                            EndpointName = "Reports.BookStatistics",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 302,
                            EndpointName = "Reports.PublicationStatistics",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 303,
                            EndpointName = "Reports.Export",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 304,
                            EndpointName = "Reports.View",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 305,
                            EndpointName = "Library.Settings.GetUserSettings",
                            RoleId = 4
                        },
                        new
                        {
                            Id = 306,
                            EndpointName = "Library.Settings.UpdateUserSettings",
                            RoleId = 4
                        },
                        new
                        {
                            Id = 307,
                            EndpointName = "Library.Settings.GetAppInfo",
                            RoleId = 4
                        },
                        new
                        {
                            Id = 308,
                            EndpointName = "Library.Users.ChangePassword",
                            RoleId = 4
                        },
                        new
                        {
                            Id = 309,
                            EndpointName = "Library.Users.UpdateProfile",
                            RoleId = 4
                        },
                        new
                        {
                            Id = 310,
                            EndpointName = "Library.Menu.GetAll",
                            RoleId = 4
                        },
                        new
                        {
                            Id = 311,
                            EndpointName = "Library.Visits.GetStats",
                            RoleId = 4
                        },
                        new
                        {
                            Id = 312,
                            EndpointName = "Library.Publications.Create",
                            RoleId = 4
                        },
                        new
                        {
                            Id = 313,
                            EndpointName = "Library.Publications.Delete",
                            RoleId = 4
                        },
                        new
                        {
                            Id = 314,
                            EndpointName = "Library.Publications.GetAll",
                            RoleId = 4
                        },
                        new
                        {
                            Id = 315,
                            EndpointName = "Library.Publications.GetAllEntities",
                            RoleId = 4
                        },
                        new
                        {
                            Id = 316,
                            EndpointName = "Library.Publications.GetById",
                            RoleId = 4
                        },
                        new
                        {
                            Id = 317,
                            EndpointName = "Library.Publications.GetCount",
                            RoleId = 4
                        },
                        new
                        {
                            Id = 318,
                            EndpointName = "Library.Publications.GetCountByLanguage",
                            RoleId = 4
                        },
                        new
                        {
                            Id = 319,
                            EndpointName = "Library.Publications.GetCountByPublishDate",
                            RoleId = 4
                        },
                        new
                        {
                            Id = 320,
                            EndpointName = "Library.Publications.GetCountByPublishPlace",
                            RoleId = 4
                        },
                        new
                        {
                            Id = 321,
                            EndpointName = "Library.Publications.GetCountBySubject",
                            RoleId = 4
                        },
                        new
                        {
                            Id = 322,
                            EndpointName = "Library.Publications.GetCountByType",
                            RoleId = 4
                        },
                        new
                        {
                            Id = 323,
                            EndpointName = "Library.Publications.GetList",
                            RoleId = 4
                        },
                        new
                        {
                            Id = 324,
                            EndpointName = "Library.Publications.GetStatistics",
                            RoleId = 4
                        },
                        new
                        {
                            Id = 325,
                            EndpointName = "Library.Publications.Search",
                            RoleId = 4
                        },
                        new
                        {
                            Id = 326,
                            EndpointName = "Library.Publications.Update",
                            RoleId = 4
                        },
                        new
                        {
                            Id = 327,
                            EndpointName = "Library.Publishers.Create",
                            RoleId = 4
                        },
                        new
                        {
                            Id = 328,
                            EndpointName = "Library.Publishers.Delete",
                            RoleId = 4
                        },
                        new
                        {
                            Id = 329,
                            EndpointName = "Library.Publishers.GetAll",
                            RoleId = 4
                        },
                        new
                        {
                            Id = 330,
                            EndpointName = "Library.Publishers.GetById",
                            RoleId = 4
                        },
                        new
                        {
                            Id = 331,
                            EndpointName = "Library.Publishers.GetList",
                            RoleId = 4
                        },
                        new
                        {
                            Id = 332,
                            EndpointName = "Library.Publishers.GetNames",
                            RoleId = 4
                        },
                        new
                        {
                            Id = 333,
                            EndpointName = "Library.Publishers.Update",
                            RoleId = 4
                        },
                        new
                        {
                            Id = 334,
                            EndpointName = "Library.PublicationTypes.Create",
                            RoleId = 4
                        },
                        new
                        {
                            Id = 335,
                            EndpointName = "Library.PublicationTypes.Delete",
                            RoleId = 4
                        },
                        new
                        {
                            Id = 336,
                            EndpointName = "Library.PublicationTypes.GetAll",
                            RoleId = 4
                        },
                        new
                        {
                            Id = 337,
                            EndpointName = "Library.PublicationTypes.GetById",
                            RoleId = 4
                        },
                        new
                        {
                            Id = 338,
                            EndpointName = "Library.PublicationTypes.GetList",
                            RoleId = 4
                        },
                        new
                        {
                            Id = 339,
                            EndpointName = "Library.PublicationTypes.GetNames",
                            RoleId = 4
                        },
                        new
                        {
                            Id = 340,
                            EndpointName = "Library.PublicationTypes.Update",
                            RoleId = 4
                        },
                        new
                        {
                            Id = 341,
                            EndpointName = "Library.Attachments.Upload",
                            RoleId = 4
                        },
                        new
                        {
                            Id = 342,
                            EndpointName = "Library.Attachments.Delete",
                            RoleId = 4
                        },
                        new
                        {
                            Id = 343,
                            EndpointName = "Library.Attachments.GetByEntity",
                            RoleId = 4
                        },
                        new
                        {
                            Id = 344,
                            EndpointName = "Library.Attachments.Update",
                            RoleId = 4
                        },
                        new
                        {
                            Id = 345,
                            EndpointName = "Reports.BookSearchList",
                            RoleId = 4
                        },
                        new
                        {
                            Id = 346,
                            EndpointName = "Reports.PublicationSearchList",
                            RoleId = 4
                        },
                        new
                        {
                            Id = 347,
                            EndpointName = "Reports.ManuscriptSearchList",
                            RoleId = 4
                        },
                        new
                        {
                            Id = 348,
                            EndpointName = "Reports.ManuscriptStatistics",
                            RoleId = 4
                        },
                        new
                        {
                            Id = 349,
                            EndpointName = "Reports.BookStatistics",
                            RoleId = 4
                        },
                        new
                        {
                            Id = 350,
                            EndpointName = "Reports.PublicationStatistics",
                            RoleId = 4
                        },
                        new
                        {
                            Id = 351,
                            EndpointName = "Reports.Export",
                            RoleId = 4
                        },
                        new
                        {
                            Id = 352,
                            EndpointName = "Reports.View",
                            RoleId = 4
                        },
                        new
                        {
                            Id = 353,
                            EndpointName = "Library.Settings.GetUserSettings",
                            RoleId = 5
                        },
                        new
                        {
                            Id = 354,
                            EndpointName = "Library.Settings.UpdateUserSettings",
                            RoleId = 5
                        },
                        new
                        {
                            Id = 355,
                            EndpointName = "Library.Settings.GetAppInfo",
                            RoleId = 5
                        },
                        new
                        {
                            Id = 356,
                            EndpointName = "Library.Users.ChangePassword",
                            RoleId = 5
                        },
                        new
                        {
                            Id = 357,
                            EndpointName = "Library.Users.UpdateProfile",
                            RoleId = 5
                        },
                        new
                        {
                            Id = 358,
                            EndpointName = "Library.Menu.GetAll",
                            RoleId = 5
                        },
                        new
                        {
                            Id = 359,
                            EndpointName = "Library.Visits.GetStats",
                            RoleId = 5
                        },
                        new
                        {
                            Id = 360,
                            EndpointName = "Library.Books.GetAll",
                            RoleId = 5
                        },
                        new
                        {
                            Id = 361,
                            EndpointName = "Library.Books.GetAllEntities",
                            RoleId = 5
                        },
                        new
                        {
                            Id = 362,
                            EndpointName = "Library.Books.GetById",
                            RoleId = 5
                        },
                        new
                        {
                            Id = 363,
                            EndpointName = "Library.Books.GetProfile",
                            RoleId = 5
                        },
                        new
                        {
                            Id = 364,
                            EndpointName = "Library.Books.GetCount",
                            RoleId = 5
                        },
                        new
                        {
                            Id = 365,
                            EndpointName = "Library.Books.GetCountByLanguage",
                            RoleId = 5
                        },
                        new
                        {
                            Id = 366,
                            EndpointName = "Library.Books.GetCountByPublisher",
                            RoleId = 5
                        },
                        new
                        {
                            Id = 367,
                            EndpointName = "Library.Books.GetCountByPublishPlace",
                            RoleId = 5
                        },
                        new
                        {
                            Id = 368,
                            EndpointName = "Library.Books.GetCountBySubject",
                            RoleId = 5
                        },
                        new
                        {
                            Id = 369,
                            EndpointName = "Library.Books.GetCountByYear",
                            RoleId = 5
                        },
                        new
                        {
                            Id = 370,
                            EndpointName = "Library.Books.GetList",
                            RoleId = 5
                        },
                        new
                        {
                            Id = 371,
                            EndpointName = "Library.Books.GetStatistics",
                            RoleId = 5
                        },
                        new
                        {
                            Id = 372,
                            EndpointName = "Library.Books.GetVolumeCount",
                            RoleId = 5
                        },
                        new
                        {
                            Id = 373,
                            EndpointName = "Library.Books.Search",
                            RoleId = 5
                        },
                        new
                        {
                            Id = 374,
                            EndpointName = "Library.Manuscripts.GetAll",
                            RoleId = 5
                        },
                        new
                        {
                            Id = 375,
                            EndpointName = "Library.Manuscripts.GetAllEntities",
                            RoleId = 5
                        },
                        new
                        {
                            Id = 376,
                            EndpointName = "Library.Manuscripts.GetById",
                            RoleId = 5
                        },
                        new
                        {
                            Id = 377,
                            EndpointName = "Library.Manuscripts.GetCount",
                            RoleId = 5
                        },
                        new
                        {
                            Id = 378,
                            EndpointName = "Library.Manuscripts.GetCountByGap",
                            RoleId = 5
                        },
                        new
                        {
                            Id = 379,
                            EndpointName = "Library.Manuscripts.GetCountByLanguage",
                            RoleId = 5
                        },
                        new
                        {
                            Id = 380,
                            EndpointName = "Library.Manuscripts.GetCountBySubject",
                            RoleId = 5
                        },
                        new
                        {
                            Id = 381,
                            EndpointName = "Library.Manuscripts.GetCountByWritingPlace",
                            RoleId = 5
                        },
                        new
                        {
                            Id = 382,
                            EndpointName = "Library.Manuscripts.GetCountByYear",
                            RoleId = 5
                        },
                        new
                        {
                            Id = 383,
                            EndpointName = "Library.Manuscripts.GetList",
                            RoleId = 5
                        },
                        new
                        {
                            Id = 384,
                            EndpointName = "Library.Manuscripts.GetStatistics",
                            RoleId = 5
                        },
                        new
                        {
                            Id = 385,
                            EndpointName = "Library.Manuscripts.Search",
                            RoleId = 5
                        },
                        new
                        {
                            Id = 386,
                            EndpointName = "Library.Publications.GetAll",
                            RoleId = 5
                        },
                        new
                        {
                            Id = 387,
                            EndpointName = "Library.Publications.GetAllEntities",
                            RoleId = 5
                        },
                        new
                        {
                            Id = 388,
                            EndpointName = "Library.Publications.GetById",
                            RoleId = 5
                        },
                        new
                        {
                            Id = 389,
                            EndpointName = "Library.Publications.GetCount",
                            RoleId = 5
                        },
                        new
                        {
                            Id = 390,
                            EndpointName = "Library.Publications.GetCountByLanguage",
                            RoleId = 5
                        },
                        new
                        {
                            Id = 391,
                            EndpointName = "Library.Publications.GetCountByPublishDate",
                            RoleId = 5
                        },
                        new
                        {
                            Id = 392,
                            EndpointName = "Library.Publications.GetCountByPublishPlace",
                            RoleId = 5
                        },
                        new
                        {
                            Id = 393,
                            EndpointName = "Library.Publications.GetCountBySubject",
                            RoleId = 5
                        },
                        new
                        {
                            Id = 394,
                            EndpointName = "Library.Publications.GetCountByType",
                            RoleId = 5
                        },
                        new
                        {
                            Id = 395,
                            EndpointName = "Library.Publications.GetList",
                            RoleId = 5
                        },
                        new
                        {
                            Id = 396,
                            EndpointName = "Library.Publications.GetStatistics",
                            RoleId = 5
                        },
                        new
                        {
                            Id = 397,
                            EndpointName = "Library.Publications.Search",
                            RoleId = 5
                        },
                        new
                        {
                            Id = 398,
                            EndpointName = "Reports.BookSearchList",
                            RoleId = 5
                        },
                        new
                        {
                            Id = 399,
                            EndpointName = "Reports.PublicationSearchList",
                            RoleId = 5
                        },
                        new
                        {
                            Id = 400,
                            EndpointName = "Reports.ManuscriptSearchList",
                            RoleId = 5
                        },
                        new
                        {
                            Id = 401,
                            EndpointName = "Reports.ManuscriptStatistics",
                            RoleId = 5
                        },
                        new
                        {
                            Id = 402,
                            EndpointName = "Reports.BookStatistics",
                            RoleId = 5
                        },
                        new
                        {
                            Id = 403,
                            EndpointName = "Reports.PublicationStatistics",
                            RoleId = 5
                        },
                        new
                        {
                            Id = 404,
                            EndpointName = "Reports.Export",
                            RoleId = 5
                        },
                        new
                        {
                            Id = 405,
                            EndpointName = "Reports.View",
                            RoleId = 5
                        },
                        new
                        {
                            Id = 406,
                            EndpointName = "Library.Attachments.GetByEntity",
                            RoleId = 5
                        });
                });

            modelBuilder.Entity("Domain.Library.Settings", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.HasKey("Id");

                    b.ToTable("Settings", "dbo");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "BackgroundImageFolder",
                            Value = "D:\\Image"
                        },
                        new
                        {
                            Id = 2,
                            Name = "CurrentBackgroundImageName",
                            Value = "library.jpg"
                        },
                        new
                        {
                            Id = 3,
                            Name = "MainTitle",
                            Value = "فرم اصلی"
                        });
                });

            modelBuilder.Entity("Domain.Library.Subject", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("Subject", "dbo");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Title = "ادبیات"
                        },
                        new
                        {
                            Id = 2,
                            Title = "تاریخ"
                        },
                        new
                        {
                            Id = 3,
                            Title = "فلسفه"
                        },
                        new
                        {
                            Id = 4,
                            Title = "علوم"
                        });
                });

            modelBuilder.Entity("Domain.Library.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("Email")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(25)
                        .HasColumnType("nvarchar(25)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("PhoneNumber")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique()
                        .HasFilter("[Email] IS NOT NULL");

                    b.HasIndex("UserName")
                        .IsUnique();

                    b.ToTable("LibraryUsers", "dbo");

                    b.HasData(
                        new
                        {
                            Id = 1L,
                            LastName = "admin",
                            Name = "admin",
                            Password = "9843417E11AFA30B39EB2E974DC2DCB2E53D4B608E665E478A1686C065107225-71815962DF6849A69BE50976ECA3DA44",
                            UserName = "admin"
                        });
                });

            modelBuilder.Entity("Domain.Library.UserRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.HasKey("Id");

                    b.HasIndex("UserId", "RoleId")
                        .IsUnique();

                    b.ToTable("UserRole", "dbo");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            RoleId = 1,
                            UserId = 1L,
                            UserName = "admin"
                        },
                        new
                        {
                            Id = 2,
                            RoleId = 2,
                            UserId = 1L,
                            UserName = "admin"
                        },
                        new
                        {
                            Id = 3,
                            RoleId = 3,
                            UserId = 1L,
                            UserName = "admin"
                        },
                        new
                        {
                            Id = 4,
                            RoleId = 4,
                            UserId = 1L,
                            UserName = "admin"
                        },
                        new
                        {
                            Id = 5,
                            RoleId = 5,
                            UserId = 1L,
                            UserName = "admin"
                        });
                });

            modelBuilder.Entity("Domain.Library.UserSettings", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<long?>("ProfileImageAttachmentId")
                        .HasColumnType("bigint");

                    b.Property<string>("ThemeAccentColor")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("ThemeMode")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("ThemePrimaryColor")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("ThemeSecondaryColor")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("ProfileImageAttachmentId");

                    b.HasIndex("UserId");

                    b.ToTable("UserSettings", "dbo");
                });

            modelBuilder.Entity("Domain.Library.VisitLog", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasMaxLength(300)
                        .HasColumnType("nvarchar(300)");

                    b.Property<DateTime>("VisitedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("VisitedAt");

                    b.ToTable("VisitLog", "dbo");
                });

            modelBuilder.Entity("Domain.Library.Book", b =>
                {
                    b.HasOne("Domain.Library.Language", "Language")
                        .WithMany("Books")
                        .HasForeignKey("LanguageId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Domain.Library.City", "City")
                        .WithMany("Books")
                        .HasForeignKey("PublishPlaceId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Domain.Library.Publisher", "Publisher")
                        .WithMany("Books")
                        .HasForeignKey("PublisherId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Domain.Library.Subject", "Subject")
                        .WithMany("Books")
                        .HasForeignKey("SubjectId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("City");

                    b.Navigation("Language");

                    b.Navigation("Publisher");

                    b.Navigation("Subject");
                });

            modelBuilder.Entity("Domain.Library.Manuscript", b =>
                {
                    b.HasOne("Domain.Library.Gap", "Gap")
                        .WithMany("Manuscripts")
                        .HasForeignKey("GapId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Domain.Library.Language", "Language")
                        .WithMany("Manuscripts")
                        .HasForeignKey("LanguageId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Domain.Library.Subject", "Subject")
                        .WithMany("Manuscripts")
                        .HasForeignKey("SubjectId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Domain.Library.City", "City")
                        .WithMany("Manuscripts")
                        .HasForeignKey("WritingPlaceId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("City");

                    b.Navigation("Gap");

                    b.Navigation("Language");

                    b.Navigation("Subject");
                });

            modelBuilder.Entity("Domain.Library.MenuItem", b =>
                {
                    b.HasOne("Domain.Library.MenuItem", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("Domain.Library.Publication", b =>
                {
                    b.HasOne("Domain.Library.Language", "Language")
                        .WithMany("Publications")
                        .HasForeignKey("LanguageId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Domain.Library.City", "City")
                        .WithMany("Publications")
                        .HasForeignKey("PublishPlaceId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Domain.Library.Subject", "Subject")
                        .WithMany("Publications")
                        .HasForeignKey("SubjectId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Domain.Library.PublicationType", "PublicationType")
                        .WithMany("Publications")
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("City");

                    b.Navigation("Language");

                    b.Navigation("PublicationType");

                    b.Navigation("Subject");
                });

            modelBuilder.Entity("Domain.Library.RoleEndpoint", b =>
                {
                    b.HasOne("Domain.Library.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");
                });

            modelBuilder.Entity("Domain.Library.UserSettings", b =>
                {
                    b.HasOne("Domain.Library.Attachment", "ProfileImageAttachment")
                        .WithMany()
                        .HasForeignKey("ProfileImageAttachmentId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("Domain.Library.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ProfileImageAttachment");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Domain.Library.City", b =>
                {
                    b.Navigation("Books");

                    b.Navigation("Manuscripts");

                    b.Navigation("Publications");
                });

            modelBuilder.Entity("Domain.Library.Gap", b =>
                {
                    b.Navigation("Manuscripts");
                });

            modelBuilder.Entity("Domain.Library.Language", b =>
                {
                    b.Navigation("Books");

                    b.Navigation("Manuscripts");

                    b.Navigation("Publications");
                });

            modelBuilder.Entity("Domain.Library.MenuItem", b =>
                {
                    b.Navigation("Children");
                });

            modelBuilder.Entity("Domain.Library.PublicationType", b =>
                {
                    b.Navigation("Publications");
                });

            modelBuilder.Entity("Domain.Library.Publisher", b =>
                {
                    b.Navigation("Books");
                });

            modelBuilder.Entity("Domain.Library.Subject", b =>
                {
                    b.Navigation("Books");

                    b.Navigation("Manuscripts");

                    b.Navigation("Publications");
                });
#pragma warning restore 612, 618
        }
    }
}
