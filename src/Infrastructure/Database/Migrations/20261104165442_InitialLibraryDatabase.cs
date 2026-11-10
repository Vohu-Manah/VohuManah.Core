using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 

namespace VohuManah.Infrastructure.Migrations
{
    
    public partial class InitialLibraryDatabase : Migration
    {
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "AppInfo",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Version = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Author = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    LastUpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Copyright = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    License = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Website = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SupportEmail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppInfo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Attachment",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    OriginalFileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EntityId = table.Column<long>(type: "bigint", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsMain = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attachment", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "City",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_City", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Gap",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    State = table.Column<bool>(type: "bit", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gap", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Language",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Abbreviation = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Language", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LibraryUsers",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LibraryUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MenuItem",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Label = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Path = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Icon = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsSystem = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenuItem_MenuItem_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "dbo",
                        principalTable: "MenuItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PublicationType",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublicationType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Publisher",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ManagerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PlaceId = table.Column<int>(type: "int", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Telephone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Website = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Publisher", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpiresAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RevokedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Settings",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Subject",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subject", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserRole",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRole", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VisitLog",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Path = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    VisitedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisitLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserSettings",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    ProfileImageAttachmentId = table.Column<long>(type: "bigint", nullable: true),
                    ThemePrimaryColor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ThemeSecondaryColor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ThemeAccentColor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ThemeMode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSettings_Attachment_ProfileImageAttachmentId",
                        column: x => x.ProfileImageAttachmentId,
                        principalSchema: "dbo",
                        principalTable: "Attachment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_UserSettings_LibraryUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "LibraryUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolePermission",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    EndpointName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolePermission_Role_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "dbo",
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Book",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Author = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Translator = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Editor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Corrector = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PublisherId = table.Column<int>(type: "int", nullable: false),
                    PublishPlaceId = table.Column<int>(type: "int", nullable: false),
                    PublishYear = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    PublishOrder = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Isbn = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    No = table.Column<string>(type: "nvarchar(800)", maxLength: 800, nullable: false),
                    VolumeCount = table.Column<int>(type: "int", nullable: false),
                    LanguageId = table.Column<int>(type: "int", nullable: false),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    BookShelfRow = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Book", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Book_City_PublishPlaceId",
                        column: x => x.PublishPlaceId,
                        principalSchema: "dbo",
                        principalTable: "City",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Book_Language_LanguageId",
                        column: x => x.LanguageId,
                        principalSchema: "dbo",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Book_Publisher_PublisherId",
                        column: x => x.PublisherId,
                        principalSchema: "dbo",
                        principalTable: "Publisher",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Book_Subject_SubjectId",
                        column: x => x.SubjectId,
                        principalSchema: "dbo",
                        principalTable: "Subject",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Manuscript",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Author = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    WritingPlaceId = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    PageCount = table.Column<int>(type: "int", nullable: false),
                    Size = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    GapId = table.Column<int>(type: "int", nullable: false),
                    VersionNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    LanguageId = table.Column<int>(type: "int", nullable: false),
                    SubjectId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Manuscript", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Manuscript_City_WritingPlaceId",
                        column: x => x.WritingPlaceId,
                        principalSchema: "dbo",
                        principalTable: "City",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Manuscript_Gap_GapId",
                        column: x => x.GapId,
                        principalSchema: "dbo",
                        principalTable: "Gap",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Manuscript_Language_LanguageId",
                        column: x => x.LanguageId,
                        principalSchema: "dbo",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Manuscript_Subject_SubjectId",
                        column: x => x.SubjectId,
                        principalSchema: "dbo",
                        principalTable: "Subject",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Publication",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TypeId = table.Column<int>(type: "int", nullable: false),
                    Concessionaire = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ResponsibleDirector = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Editor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Year = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    JournalNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PublishDate = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PublishPlaceId = table.Column<int>(type: "int", nullable: false),
                    No = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Period = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    LanguageId = table.Column<int>(type: "int", nullable: false),
                    SubjectId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Publication", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Publication_City_PublishPlaceId",
                        column: x => x.PublishPlaceId,
                        principalSchema: "dbo",
                        principalTable: "City",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Publication_Language_LanguageId",
                        column: x => x.LanguageId,
                        principalSchema: "dbo",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Publication_PublicationType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "dbo",
                        principalTable: "PublicationType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Publication_Subject_SubjectId",
                        column: x => x.SubjectId,
                        principalSchema: "dbo",
                        principalTable: "Subject",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                schema: "dbo",
                table: "AppInfo",
                columns: new[] { "Id", "AppName", "Author", "Copyright", "Description", "LastUpdateDate", "License", "SupportEmail", "Version", "Website" },
                values: new object[] { 1, "Vohu Manah", "Vohu Manah Team", "© 2025 Vohu Manah", "سیستم مدیریت کتابخانه", null, "Proprietary", "support@vohumanah.com", "1.0.0", "https://vohumanah.com" });

            migrationBuilder.InsertData(
                schema: "dbo",
                table: "City",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "تهران" },
                    { 2, "اصفهان" },
                    { 3, "شیراز" }
                });

            migrationBuilder.InsertData(
                schema: "dbo",
                table: "Gap",
                columns: new[] { "Id", "Note", "State", "Title" },
                values: new object[,]
                {
                    { 1, "توضیحات گپ 1", true, "گپ 1" },
                    { 2, "توضیحات گپ 2", false, "گپ 2" }
                });

            migrationBuilder.InsertData(
                schema: "dbo",
                table: "Language",
                columns: new[] { "Id", "Abbreviation", "Name" },
                values: new object[,]
                {
                    { 1, "FA", "فارسی" },
                    { 2, "EN", "English" },
                    { 3, "AR", "العربية" }
                });

            migrationBuilder.InsertData(
                schema: "dbo",
                table: "LibraryUsers",
                columns: new[] { "Id", "Email", "LastName", "Name", "Password", "PhoneNumber", "UserName" },
                values: new object[] { 1L, null, "admin", "admin", "9843417E11AFA30B39EB2E974DC2DCB2E53D4B608E665E478A1686C065107225-71815962DF6849A69BE50976ECA3DA44", null, "admin" });

            migrationBuilder.InsertData(
                schema: "dbo",
                table: "MenuItem",
                columns: new[] { "Id", "DisplayOrder", "Icon", "IsActive", "IsSystem", "Label", "ParentId", "Path" },
                values: new object[,]
                {
                    { 1, 0, "FaHome", true, false, "داشبورد", null, "/" },
                    { 2, 1, "FaBook", true, false, "کتابها", null, null },
                    { 6, 2, "FaNewspaper", true, false, "نشریات", null, null },
                    { 10, 3, "FaScroll", true, false, "نسخ خطی", null, null },
                    { 14, 4, "FaTags", true, false, "اطلاعات پایه", null, null },
                    { 21, 5, "FaUsers", true, false, "کاربران", null, null },
                    { 23, 6, "FaUser", true, false, "پروفایل", null, "/profile" },
                    { 24, 7, "FaFileAlt", true, false, "گزارشها", null, null },
                    { 31, 8, "FaCog", true, false, "تنظیمات", null, null },
                    { 34, 9, "FaShieldAlt", true, false, "مدیریت ادمین", null, null }
                });

            migrationBuilder.InsertData(
                schema: "dbo",
                table: "PublicationType",
                columns: new[] { "Id", "Title" },
                values: new object[,]
                {
                    { 1, "مجله" },
                    { 2, "روزنامه" },
                    { 3, "نشریه" }
                });

            migrationBuilder.InsertData(
                schema: "dbo",
                table: "Publisher",
                columns: new[] { "Id", "Address", "Email", "ManagerName", "Name", "PlaceId", "Telephone", "Website" },
                values: new object[] { 1, "تهران، خیابان انقلاب", "info@amirkabir.com", "مدیر انتشارات", "انتشارات امیرکبیر", 1, "021-12345678", "www.amirkabir.com" });

            migrationBuilder.InsertData(
                schema: "dbo",
                table: "Role",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Admin" },
                    { 2, "User" },
                    { 3, "Librarian" },
                    { 4, "Publisher" },
                    { 5, "Author" }
                });

            migrationBuilder.InsertData(
                schema: "dbo",
                table: "Settings",
                columns: new[] { "Id", "Name", "Value" },
                values: new object[,]
                {
                    { 1, "BackgroundImageFolder", "D:\\Image" },
                    { 2, "CurrentBackgroundImageName", "library.jpg" },
                    { 3, "MainTitle", "فرم اصلی" }
                });

            migrationBuilder.InsertData(
                schema: "dbo",
                table: "Subject",
                columns: new[] { "Id", "Title" },
                values: new object[,]
                {
                    { 1, "ادبیات" },
                    { 2, "تاریخ" },
                    { 3, "فلسفه" },
                    { 4, "علوم" }
                });

            migrationBuilder.InsertData(
                schema: "dbo",
                table: "UserRole",
                columns: new[] { "Id", "RoleId", "UserId", "UserName" },
                values: new object[,]
                {
                    { 1, 1, 1L, "admin" },
                    { 2, 2, 1L, "admin" },
                    { 3, 3, 1L, "admin" },
                    { 4, 4, 1L, "admin" },
                    { 5, 5, 1L, "admin" }
                });

            migrationBuilder.InsertData(
                schema: "dbo",
                table: "MenuItem",
                columns: new[] { "Id", "DisplayOrder", "Icon", "IsActive", "IsSystem", "Label", "ParentId", "Path" },
                values: new object[,]
                {
                    { 3, 0, null, true, false, "مدیریت کتابها", 2, "/books" },
                    { 4, 1, null, true, false, "جستجوی کتاب", 2, "/books/search" },
                    { 5, 2, null, true, false, "آمار کتابها", 2, "/books/statistics" },
                    { 7, 0, null, true, false, "مدیریت نشریات", 6, "/publications" },
                    { 8, 1, null, true, false, "جستجوی نشریه", 6, "/publications/search" },
                    { 9, 2, null, true, false, "آمار نشریات", 6, "/publications/statistics" },
                    { 11, 0, null, true, false, "مدیریت نسخ خطی", 10, "/manuscripts" },
                    { 12, 1, null, true, false, "جستجوی نسخه خطی", 10, "/manuscripts/search" },
                    { 13, 2, null, true, false, "آمار نسخ خطی", 10, "/manuscripts/statistics" },
                    { 15, 0, null, true, false, "شهرها", 14, "/cities" },
                    { 16, 1, null, true, false, "زبانها", 14, "/languages" },
                    { 17, 2, null, true, false, "موضوعات", 14, "/subjects" },
                    { 18, 3, null, true, false, "ناشران", 14, "/publishers" },
                    { 19, 4, null, true, false, "فاصلهها", 14, "/gaps" },
                    { 20, 5, null, true, false, "انواع نشریات", 14, "/publication-types" },
                    { 22, 0, null, true, false, "مدیریت کاربران", 21, "/users" },
                    { 25, 0, null, true, false, "جستجوی کتاب", 24, "/books/search" },
                    { 26, 1, null, true, false, "جستجوی نسخه خطی", 24, "/manuscripts/search" },
                    { 27, 2, null, true, false, "جستجوی نشریه", 24, "/publications/search" },
                    { 28, 3, null, true, false, "گزارشات آماری کتاب", 24, "/books/statistics" },
                    { 29, 4, null, true, false, "گزارشات آماری نسخ خطی", 24, "/manuscripts/statistics" },
                    { 30, 5, null, true, false, "گزارشات آماری نشریه", 24, "/publications/statistics" },
                    { 32, 0, null, true, false, "تنظیمات عمومی", 31, "/settings" },
                    { 33, 1, null, true, true, "تنظیمات منو", 31, "/menu-settings" },
                    { 35, 0, null, true, false, "مدیریت نقشها", 34, "/admin/roles" },
                    { 36, 1, null, true, false, "نقشدهی به کاربران", 34, "/admin/user-roles" }
                });

            migrationBuilder.InsertData(
                schema: "dbo",
                table: "RolePermission",
                columns: new[] { "Id", "EndpointName", "RoleId" },
                values: new object[,]
                {
                    { 1, "Library.Attachments.Delete", 1 },
                    { 2, "Library.Attachments.GetByEntity", 1 },
                    { 3, "Library.Attachments.Update", 1 },
                    { 4, "Library.Attachments.Upload", 1 },
                    { 5, "Library.Backup.Create", 1 },
                    { 6, "Library.Backup.Download", 1 },
                    { 7, "Library.Backup.GetInfo", 1 },
                    { 8, "Library.Backup.Restore", 1 },
                    { 9, "Library.Books.Create", 1 },
                    { 10, "Library.Books.Delete", 1 },
                    { 11, "Library.Books.GetAll", 1 },
                    { 12, "Library.Books.GetAllEntities", 1 },
                    { 13, "Library.Books.GetById", 1 },
                    { 14, "Library.Books.GetCount", 1 },
                    { 15, "Library.Books.GetCountByLanguage", 1 },
                    { 16, "Library.Books.GetCountByPublisher", 1 },
                    { 17, "Library.Books.GetCountByPublishPlace", 1 },
                    { 18, "Library.Books.GetCountBySubject", 1 },
                    { 19, "Library.Books.GetCountByYear", 1 },
                    { 20, "Library.Books.GetList", 1 },
                    { 21, "Library.Books.GetProfile", 1 },
                    { 22, "Library.Books.GetStatistics", 1 },
                    { 23, "Library.Books.GetVolumeCount", 1 },
                    { 24, "Library.Books.Search", 1 },
                    { 25, "Library.Books.Update", 1 },
                    { 26, "Library.Cities.Create", 1 },
                    { 27, "Library.Cities.Delete", 1 },
                    { 28, "Library.Cities.GetAll", 1 },
                    { 29, "Library.Cities.GetById", 1 },
                    { 30, "Library.Cities.GetList", 1 },
                    { 31, "Library.Cities.GetNames", 1 },
                    { 32, "Library.Cities.Update", 1 },
                    { 33, "Library.Gaps.Create", 1 },
                    { 34, "Library.Gaps.Delete", 1 },
                    { 35, "Library.Gaps.GetAll", 1 },
                    { 36, "Library.Gaps.GetById", 1 },
                    { 37, "Library.Gaps.GetList", 1 },
                    { 38, "Library.Gaps.GetNames", 1 },
                    { 39, "Library.Gaps.Update", 1 },
                    { 40, "Library.Languages.Create", 1 },
                    { 41, "Library.Languages.Delete", 1 },
                    { 42, "Library.Languages.GetAll", 1 },
                    { 43, "Library.Languages.GetById", 1 },
                    { 44, "Library.Languages.GetList", 1 },
                    { 45, "Library.Languages.GetNames", 1 },
                    { 46, "Library.Languages.Update", 1 },
                    { 47, "Library.Manuscripts.Create", 1 },
                    { 48, "Library.Manuscripts.Delete", 1 },
                    { 49, "Library.Manuscripts.GetAll", 1 },
                    { 50, "Library.Manuscripts.GetAllEntities", 1 },
                    { 51, "Library.Manuscripts.GetById", 1 },
                    { 52, "Library.Manuscripts.GetCount", 1 },
                    { 53, "Library.Manuscripts.GetCountByGap", 1 },
                    { 54, "Library.Manuscripts.GetCountByLanguage", 1 },
                    { 55, "Library.Manuscripts.GetCountBySubject", 1 },
                    { 56, "Library.Manuscripts.GetCountByWritingPlace", 1 },
                    { 57, "Library.Manuscripts.GetCountByYear", 1 },
                    { 58, "Library.Manuscripts.GetList", 1 },
                    { 59, "Library.Manuscripts.GetStatistics", 1 },
                    { 60, "Library.Manuscripts.Search", 1 },
                    { 61, "Library.Manuscripts.Update", 1 },
                    { 62, "Library.Menu.Create", 1 },
                    { 63, "Library.Menu.Delete", 1 },
                    { 64, "Library.Menu.GetAll", 1 },
                    { 65, "Library.Menu.Reorder", 1 },
                    { 66, "Library.Menu.ResetToDefault", 1 },
                    { 67, "Library.Menu.Update", 1 },
                    { 68, "Library.Publications.Create", 1 },
                    { 69, "Library.Publications.Delete", 1 },
                    { 70, "Library.Publications.GetAll", 1 },
                    { 71, "Library.Publications.GetAllEntities", 1 },
                    { 72, "Library.Publications.GetById", 1 },
                    { 73, "Library.Publications.GetCount", 1 },
                    { 74, "Library.Publications.GetCountByLanguage", 1 },
                    { 75, "Library.Publications.GetCountByPublishDate", 1 },
                    { 76, "Library.Publications.GetCountByPublishPlace", 1 },
                    { 77, "Library.Publications.GetCountBySubject", 1 },
                    { 78, "Library.Publications.GetCountByType", 1 },
                    { 79, "Library.Publications.GetList", 1 },
                    { 80, "Library.Publications.GetStatistics", 1 },
                    { 81, "Library.Publications.Search", 1 },
                    { 82, "Library.Publications.Update", 1 },
                    { 83, "Library.PublicationTypes.Create", 1 },
                    { 84, "Library.PublicationTypes.Delete", 1 },
                    { 85, "Library.PublicationTypes.GetAll", 1 },
                    { 86, "Library.PublicationTypes.GetById", 1 },
                    { 87, "Library.PublicationTypes.GetList", 1 },
                    { 88, "Library.PublicationTypes.GetNames", 1 },
                    { 89, "Library.PublicationTypes.Update", 1 },
                    { 90, "Library.Publishers.Create", 1 },
                    { 91, "Library.Publishers.Delete", 1 },
                    { 92, "Library.Publishers.GetAll", 1 },
                    { 93, "Library.Publishers.GetById", 1 },
                    { 94, "Library.Publishers.GetList", 1 },
                    { 95, "Library.Publishers.GetNames", 1 },
                    { 96, "Library.Publishers.Update", 1 },
                    { 97, "Library.Settings.AssignRoleToUser", 1 },
                    { 98, "Library.Settings.CreateRole", 1 },
                    { 99, "Library.Settings.GetAll", 1 },
                    { 100, "Library.Settings.GetAllEndpoints", 1 },
                    { 101, "Library.Settings.GetAllEntities", 1 },
                    { 102, "Library.Settings.GetAllRoles", 1 },
                    { 103, "Library.Settings.GetAppInfo", 1 },
                    { 104, "Library.Settings.GetBackground", 1 },
                    { 105, "Library.Settings.GetById", 1 },
                    { 106, "Library.Settings.GetByName", 1 },
                    { 107, "Library.Settings.GetMainTitle", 1 },
                    { 108, "Library.Settings.GetRolePermissions", 1 },
                    { 109, "Library.Settings.GetSystemInfo", 1 },
                    { 110, "Library.Settings.GetUserRoles", 1 },
                    { 111, "Library.Settings.GetUserSettings", 1 },
                    { 112, "Library.Settings.RemoveRoleFromUser", 1 },
                    { 113, "Library.Settings.Update", 1 },
                    { 114, "Library.Settings.UpdateRoleEndpoints", 1 },
                    { 115, "Library.Settings.UpdateUserSettings", 1 },
                    { 116, "Library.Subjects.Create", 1 },
                    { 117, "Library.Subjects.Delete", 1 },
                    { 118, "Library.Subjects.GetAll", 1 },
                    { 119, "Library.Subjects.GetById", 1 },
                    { 120, "Library.Subjects.GetList", 1 },
                    { 121, "Library.Subjects.GetNames", 1 },
                    { 122, "Library.Subjects.Update", 1 },
                    { 123, "Library.Users.ChangePassword", 1 },
                    { 124, "Library.Users.Create", 1 },
                    { 125, "Library.Users.Delete", 1 },
                    { 126, "Library.Users.GetAll", 1 },
                    { 127, "Library.Users.GetAllEntities", 1 },
                    { 128, "Library.Users.GetByUserName", 1 },
                    { 129, "Library.Users.GetFullName", 1 },
                    { 130, "Library.Users.GetList", 1 },
                    { 131, "Library.Users.Revoke", 1 },
                    { 132, "Library.Users.Update", 1 },
                    { 133, "Library.Users.UpdateProfile", 1 },
                    { 134, "Library.Visits.GetStats", 1 },
                    { 135, "Reports.BookSearchList", 1 },
                    { 136, "Reports.BookStatistics", 1 },
                    { 137, "Reports.Export", 1 },
                    { 138, "Reports.ManuscriptSearchList", 1 },
                    { 139, "Reports.ManuscriptStatistics", 1 },
                    { 140, "Reports.PublicationSearchList", 1 },
                    { 141, "Reports.PublicationStatistics", 1 },
                    { 142, "Reports.View", 1 },
                    { 143, "Library.Settings.GetUserSettings", 2 },
                    { 144, "Library.Settings.UpdateUserSettings", 2 },
                    { 145, "Library.Settings.GetAppInfo", 2 },
                    { 146, "Library.Users.ChangePassword", 2 },
                    { 147, "Library.Users.UpdateProfile", 2 },
                    { 148, "Library.Menu.GetAll", 2 },
                    { 149, "Library.Visits.GetStats", 2 },
                    { 150, "Library.Books.GetAll", 2 },
                    { 151, "Library.Books.GetAllEntities", 2 },
                    { 152, "Library.Books.GetById", 2 },
                    { 153, "Library.Books.GetProfile", 2 },
                    { 154, "Library.Books.GetCount", 2 },
                    { 155, "Library.Books.GetCountByLanguage", 2 },
                    { 156, "Library.Books.GetCountByPublisher", 2 },
                    { 157, "Library.Books.GetCountByPublishPlace", 2 },
                    { 158, "Library.Books.GetCountBySubject", 2 },
                    { 159, "Library.Books.GetCountByYear", 2 },
                    { 160, "Library.Books.GetList", 2 },
                    { 161, "Library.Books.GetStatistics", 2 },
                    { 162, "Library.Books.GetVolumeCount", 2 },
                    { 163, "Library.Books.Search", 2 },
                    { 164, "Library.Manuscripts.GetAll", 2 },
                    { 165, "Library.Manuscripts.GetAllEntities", 2 },
                    { 166, "Library.Manuscripts.GetById", 2 },
                    { 167, "Library.Manuscripts.GetCount", 2 },
                    { 168, "Library.Manuscripts.GetCountByGap", 2 },
                    { 169, "Library.Manuscripts.GetCountByLanguage", 2 },
                    { 170, "Library.Manuscripts.GetCountBySubject", 2 },
                    { 171, "Library.Manuscripts.GetCountByWritingPlace", 2 },
                    { 172, "Library.Manuscripts.GetCountByYear", 2 },
                    { 173, "Library.Manuscripts.GetList", 2 },
                    { 174, "Library.Manuscripts.GetStatistics", 2 },
                    { 175, "Library.Manuscripts.Search", 2 },
                    { 176, "Library.Publications.GetAll", 2 },
                    { 177, "Library.Publications.GetAllEntities", 2 },
                    { 178, "Library.Publications.GetById", 2 },
                    { 179, "Library.Publications.GetCount", 2 },
                    { 180, "Library.Publications.GetCountByLanguage", 2 },
                    { 181, "Library.Publications.GetCountByPublishDate", 2 },
                    { 182, "Library.Publications.GetCountByPublishPlace", 2 },
                    { 183, "Library.Publications.GetCountBySubject", 2 },
                    { 184, "Library.Publications.GetCountByType", 2 },
                    { 185, "Library.Publications.GetList", 2 },
                    { 186, "Library.Publications.GetStatistics", 2 },
                    { 187, "Library.Publications.Search", 2 },
                    { 188, "Library.Settings.GetUserSettings", 3 },
                    { 189, "Library.Settings.UpdateUserSettings", 3 },
                    { 190, "Library.Settings.GetAppInfo", 3 },
                    { 191, "Library.Users.ChangePassword", 3 },
                    { 192, "Library.Users.UpdateProfile", 3 },
                    { 193, "Library.Menu.GetAll", 3 },
                    { 194, "Library.Visits.GetStats", 3 },
                    { 195, "Library.Books.Create", 3 },
                    { 196, "Library.Books.Delete", 3 },
                    { 197, "Library.Books.GetAll", 3 },
                    { 198, "Library.Books.GetAllEntities", 3 },
                    { 199, "Library.Books.GetById", 3 },
                    { 200, "Library.Books.GetProfile", 3 },
                    { 201, "Library.Books.GetCount", 3 },
                    { 202, "Library.Books.GetCountByLanguage", 3 },
                    { 203, "Library.Books.GetCountByPublisher", 3 },
                    { 204, "Library.Books.GetCountByPublishPlace", 3 },
                    { 205, "Library.Books.GetCountBySubject", 3 },
                    { 206, "Library.Books.GetCountByYear", 3 },
                    { 207, "Library.Books.GetList", 3 },
                    { 208, "Library.Books.GetStatistics", 3 },
                    { 209, "Library.Books.GetVolumeCount", 3 },
                    { 210, "Library.Books.Search", 3 },
                    { 211, "Library.Books.Update", 3 },
                    { 212, "Library.Manuscripts.Create", 3 },
                    { 213, "Library.Manuscripts.Delete", 3 },
                    { 214, "Library.Manuscripts.GetAll", 3 },
                    { 215, "Library.Manuscripts.GetAllEntities", 3 },
                    { 216, "Library.Manuscripts.GetById", 3 },
                    { 217, "Library.Manuscripts.GetCount", 3 },
                    { 218, "Library.Manuscripts.GetCountByGap", 3 },
                    { 219, "Library.Manuscripts.GetCountByLanguage", 3 },
                    { 220, "Library.Manuscripts.GetCountBySubject", 3 },
                    { 221, "Library.Manuscripts.GetCountByWritingPlace", 3 },
                    { 222, "Library.Manuscripts.GetCountByYear", 3 },
                    { 223, "Library.Manuscripts.GetList", 3 },
                    { 224, "Library.Manuscripts.GetStatistics", 3 },
                    { 225, "Library.Manuscripts.Search", 3 },
                    { 226, "Library.Manuscripts.Update", 3 },
                    { 227, "Library.Publications.Create", 3 },
                    { 228, "Library.Publications.Delete", 3 },
                    { 229, "Library.Publications.GetAll", 3 },
                    { 230, "Library.Publications.GetAllEntities", 3 },
                    { 231, "Library.Publications.GetById", 3 },
                    { 232, "Library.Publications.GetCount", 3 },
                    { 233, "Library.Publications.GetCountByLanguage", 3 },
                    { 234, "Library.Publications.GetCountByPublishDate", 3 },
                    { 235, "Library.Publications.GetCountByPublishPlace", 3 },
                    { 236, "Library.Publications.GetCountBySubject", 3 },
                    { 237, "Library.Publications.GetCountByType", 3 },
                    { 238, "Library.Publications.GetList", 3 },
                    { 239, "Library.Publications.GetStatistics", 3 },
                    { 240, "Library.Publications.Search", 3 },
                    { 241, "Library.Publications.Update", 3 },
                    { 242, "Library.PublicationTypes.Create", 3 },
                    { 243, "Library.PublicationTypes.Delete", 3 },
                    { 244, "Library.PublicationTypes.GetAll", 3 },
                    { 245, "Library.PublicationTypes.GetById", 3 },
                    { 246, "Library.PublicationTypes.GetList", 3 },
                    { 247, "Library.PublicationTypes.GetNames", 3 },
                    { 248, "Library.PublicationTypes.Update", 3 },
                    { 249, "Library.Publishers.Create", 3 },
                    { 250, "Library.Publishers.Delete", 3 },
                    { 251, "Library.Publishers.GetAll", 3 },
                    { 252, "Library.Publishers.GetById", 3 },
                    { 253, "Library.Publishers.GetList", 3 },
                    { 254, "Library.Publishers.GetNames", 3 },
                    { 255, "Library.Publishers.Update", 3 },
                    { 256, "Library.Languages.Create", 3 },
                    { 257, "Library.Languages.Delete", 3 },
                    { 258, "Library.Languages.GetAll", 3 },
                    { 259, "Library.Languages.GetById", 3 },
                    { 260, "Library.Languages.GetList", 3 },
                    { 261, "Library.Languages.GetNames", 3 },
                    { 262, "Library.Languages.Update", 3 },
                    { 263, "Library.Cities.Create", 3 },
                    { 264, "Library.Cities.Delete", 3 },
                    { 265, "Library.Cities.GetAll", 3 },
                    { 266, "Library.Cities.GetById", 3 },
                    { 267, "Library.Cities.GetList", 3 },
                    { 268, "Library.Cities.GetNames", 3 },
                    { 269, "Library.Cities.Update", 3 },
                    { 270, "Library.Subjects.Create", 3 },
                    { 271, "Library.Subjects.Delete", 3 },
                    { 272, "Library.Subjects.GetAll", 3 },
                    { 273, "Library.Subjects.GetById", 3 },
                    { 274, "Library.Subjects.GetList", 3 },
                    { 275, "Library.Subjects.GetNames", 3 },
                    { 276, "Library.Subjects.Update", 3 },
                    { 277, "Library.Gaps.Create", 3 },
                    { 278, "Library.Gaps.Delete", 3 },
                    { 279, "Library.Gaps.GetAll", 3 },
                    { 280, "Library.Gaps.GetById", 3 },
                    { 281, "Library.Gaps.GetList", 3 },
                    { 282, "Library.Gaps.GetNames", 3 },
                    { 283, "Library.Gaps.Update", 3 },
                    { 284, "Library.Attachments.Upload", 3 },
                    { 285, "Library.Attachments.Delete", 3 },
                    { 286, "Library.Attachments.GetByEntity", 3 },
                    { 287, "Library.Attachments.Update", 3 },
                    { 288, "Reports.BookSearchList", 3 },
                    { 289, "Reports.PublicationSearchList", 3 },
                    { 290, "Reports.ManuscriptSearchList", 3 },
                    { 291, "Reports.ManuscriptStatistics", 3 },
                    { 292, "Reports.BookStatistics", 3 },
                    { 293, "Reports.PublicationStatistics", 3 },
                    { 294, "Reports.Export", 3 },
                    { 295, "Reports.View", 3 },
                    { 296, "Library.Settings.GetUserSettings", 4 },
                    { 297, "Library.Settings.UpdateUserSettings", 4 },
                    { 298, "Library.Settings.GetAppInfo", 4 },
                    { 299, "Library.Users.ChangePassword", 4 },
                    { 300, "Library.Users.UpdateProfile", 4 },
                    { 301, "Library.Menu.GetAll", 4 },
                    { 302, "Library.Visits.GetStats", 4 },
                    { 303, "Library.Publications.Create", 4 },
                    { 304, "Library.Publications.Delete", 4 },
                    { 305, "Library.Publications.GetAll", 4 },
                    { 306, "Library.Publications.GetAllEntities", 4 },
                    { 307, "Library.Publications.GetById", 4 },
                    { 308, "Library.Publications.GetCount", 4 },
                    { 309, "Library.Publications.GetCountByLanguage", 4 },
                    { 310, "Library.Publications.GetCountByPublishDate", 4 },
                    { 311, "Library.Publications.GetCountByPublishPlace", 4 },
                    { 312, "Library.Publications.GetCountBySubject", 4 },
                    { 313, "Library.Publications.GetCountByType", 4 },
                    { 314, "Library.Publications.GetList", 4 },
                    { 315, "Library.Publications.GetStatistics", 4 },
                    { 316, "Library.Publications.Search", 4 },
                    { 317, "Library.Publications.Update", 4 },
                    { 318, "Library.Publishers.Create", 4 },
                    { 319, "Library.Publishers.Delete", 4 },
                    { 320, "Library.Publishers.GetAll", 4 },
                    { 321, "Library.Publishers.GetById", 4 },
                    { 322, "Library.Publishers.GetList", 4 },
                    { 323, "Library.Publishers.GetNames", 4 },
                    { 324, "Library.Publishers.Update", 4 },
                    { 325, "Library.PublicationTypes.Create", 4 },
                    { 326, "Library.PublicationTypes.Delete", 4 },
                    { 327, "Library.PublicationTypes.GetAll", 4 },
                    { 328, "Library.PublicationTypes.GetById", 4 },
                    { 329, "Library.PublicationTypes.GetList", 4 },
                    { 330, "Library.PublicationTypes.GetNames", 4 },
                    { 331, "Library.PublicationTypes.Update", 4 },
                    { 332, "Library.Attachments.Upload", 4 },
                    { 333, "Library.Attachments.Delete", 4 },
                    { 334, "Library.Attachments.GetByEntity", 4 },
                    { 335, "Library.Attachments.Update", 4 },
                    { 336, "Library.Settings.GetUserSettings", 5 },
                    { 337, "Library.Settings.UpdateUserSettings", 5 },
                    { 338, "Library.Settings.GetAppInfo", 5 },
                    { 339, "Library.Users.ChangePassword", 5 },
                    { 340, "Library.Users.UpdateProfile", 5 },
                    { 341, "Library.Menu.GetAll", 5 },
                    { 342, "Library.Visits.GetStats", 5 },
                    { 343, "Library.Books.GetAll", 5 },
                    { 344, "Library.Books.GetAllEntities", 5 },
                    { 345, "Library.Books.GetById", 5 },
                    { 346, "Library.Books.GetProfile", 5 },
                    { 347, "Library.Books.GetCount", 5 },
                    { 348, "Library.Books.GetCountByLanguage", 5 },
                    { 349, "Library.Books.GetCountByPublisher", 5 },
                    { 350, "Library.Books.GetCountByPublishPlace", 5 },
                    { 351, "Library.Books.GetCountBySubject", 5 },
                    { 352, "Library.Books.GetCountByYear", 5 },
                    { 353, "Library.Books.GetList", 5 },
                    { 354, "Library.Books.GetStatistics", 5 },
                    { 355, "Library.Books.GetVolumeCount", 5 },
                    { 356, "Library.Books.Search", 5 },
                    { 357, "Library.Manuscripts.GetAll", 5 },
                    { 358, "Library.Manuscripts.GetAllEntities", 5 },
                    { 359, "Library.Manuscripts.GetById", 5 },
                    { 360, "Library.Manuscripts.GetCount", 5 },
                    { 361, "Library.Manuscripts.GetCountByGap", 5 },
                    { 362, "Library.Manuscripts.GetCountByLanguage", 5 },
                    { 363, "Library.Manuscripts.GetCountBySubject", 5 },
                    { 364, "Library.Manuscripts.GetCountByWritingPlace", 5 },
                    { 365, "Library.Manuscripts.GetCountByYear", 5 },
                    { 366, "Library.Manuscripts.GetList", 5 },
                    { 367, "Library.Manuscripts.GetStatistics", 5 },
                    { 368, "Library.Manuscripts.Search", 5 },
                    { 369, "Library.Publications.GetAll", 5 },
                    { 370, "Library.Publications.GetAllEntities", 5 },
                    { 371, "Library.Publications.GetById", 5 },
                    { 372, "Library.Publications.GetCount", 5 },
                    { 373, "Library.Publications.GetCountByLanguage", 5 },
                    { 374, "Library.Publications.GetCountByPublishDate", 5 },
                    { 375, "Library.Publications.GetCountByPublishPlace", 5 },
                    { 376, "Library.Publications.GetCountBySubject", 5 },
                    { 377, "Library.Publications.GetCountByType", 5 },
                    { 378, "Library.Publications.GetList", 5 },
                    { 379, "Library.Publications.GetStatistics", 5 },
                    { 380, "Library.Publications.Search", 5 }
                });

            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 188,
                columns: new[] { "EndpointName", "RoleId" },
                values: new object[] { "Reports.BookSearchList", 2 });
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 189,
                columns: new[] { "EndpointName", "RoleId" },
                values: new object[] { "Reports.PublicationSearchList", 2 });
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 190,
                columns: new[] { "EndpointName", "RoleId" },
                values: new object[] { "Reports.ManuscriptSearchList", 2 });
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 191,
                columns: new[] { "EndpointName", "RoleId" },
                values: new object[] { "Reports.ManuscriptStatistics", 2 });
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 192,
                columns: new[] { "EndpointName", "RoleId" },
                values: new object[] { "Reports.BookStatistics", 2 });
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 193,
                columns: new[] { "EndpointName", "RoleId" },
                values: new object[] { "Reports.PublicationStatistics", 2 });
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 194,
                columns: new[] { "EndpointName", "RoleId" },
                values: new object[] { "Reports.Export", 2 });
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 195,
                columns: new[] { "EndpointName", "RoleId" },
                values: new object[] { "Reports.View", 2 });
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 196,
                columns: new[] { "EndpointName", "RoleId" },
                values: new object[] { "Library.Attachments.GetByEntity", 2 });
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 197,
                column: "EndpointName",
                value: "Library.Settings.GetUserSettings");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 198,
                column: "EndpointName",
                value: "Library.Settings.UpdateUserSettings");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 199,
                column: "EndpointName",
                value: "Library.Settings.GetAppInfo");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 200,
                column: "EndpointName",
                value: "Library.Users.ChangePassword");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 201,
                column: "EndpointName",
                value: "Library.Users.UpdateProfile");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 202,
                column: "EndpointName",
                value: "Library.Menu.GetAll");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 203,
                column: "EndpointName",
                value: "Library.Visits.GetStats");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 204,
                column: "EndpointName",
                value: "Library.Books.Create");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 205,
                column: "EndpointName",
                value: "Library.Books.Delete");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 206,
                column: "EndpointName",
                value: "Library.Books.GetAll");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 207,
                column: "EndpointName",
                value: "Library.Books.GetAllEntities");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 208,
                column: "EndpointName",
                value: "Library.Books.GetById");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 209,
                column: "EndpointName",
                value: "Library.Books.GetProfile");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 210,
                column: "EndpointName",
                value: "Library.Books.GetCount");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 211,
                column: "EndpointName",
                value: "Library.Books.GetCountByLanguage");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 212,
                column: "EndpointName",
                value: "Library.Books.GetCountByPublisher");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 213,
                column: "EndpointName",
                value: "Library.Books.GetCountByPublishPlace");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 214,
                column: "EndpointName",
                value: "Library.Books.GetCountBySubject");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 215,
                column: "EndpointName",
                value: "Library.Books.GetCountByYear");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 216,
                column: "EndpointName",
                value: "Library.Books.GetList");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 217,
                column: "EndpointName",
                value: "Library.Books.GetStatistics");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 218,
                column: "EndpointName",
                value: "Library.Books.GetVolumeCount");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 219,
                column: "EndpointName",
                value: "Library.Books.Search");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 220,
                column: "EndpointName",
                value: "Library.Books.Update");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 221,
                column: "EndpointName",
                value: "Library.Manuscripts.Create");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 222,
                column: "EndpointName",
                value: "Library.Manuscripts.Delete");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 223,
                column: "EndpointName",
                value: "Library.Manuscripts.GetAll");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 224,
                column: "EndpointName",
                value: "Library.Manuscripts.GetAllEntities");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 225,
                column: "EndpointName",
                value: "Library.Manuscripts.GetById");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 226,
                column: "EndpointName",
                value: "Library.Manuscripts.GetCount");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 227,
                column: "EndpointName",
                value: "Library.Manuscripts.GetCountByGap");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 228,
                column: "EndpointName",
                value: "Library.Manuscripts.GetCountByLanguage");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 229,
                column: "EndpointName",
                value: "Library.Manuscripts.GetCountBySubject");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 230,
                column: "EndpointName",
                value: "Library.Manuscripts.GetCountByWritingPlace");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 231,
                column: "EndpointName",
                value: "Library.Manuscripts.GetCountByYear");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 232,
                column: "EndpointName",
                value: "Library.Manuscripts.GetList");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 233,
                column: "EndpointName",
                value: "Library.Manuscripts.GetStatistics");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 234,
                column: "EndpointName",
                value: "Library.Manuscripts.Search");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 235,
                column: "EndpointName",
                value: "Library.Manuscripts.Update");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 236,
                column: "EndpointName",
                value: "Library.Publications.Create");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 237,
                column: "EndpointName",
                value: "Library.Publications.Delete");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 238,
                column: "EndpointName",
                value: "Library.Publications.GetAll");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 239,
                column: "EndpointName",
                value: "Library.Publications.GetAllEntities");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 240,
                column: "EndpointName",
                value: "Library.Publications.GetById");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 241,
                column: "EndpointName",
                value: "Library.Publications.GetCount");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 242,
                column: "EndpointName",
                value: "Library.Publications.GetCountByLanguage");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 243,
                column: "EndpointName",
                value: "Library.Publications.GetCountByPublishDate");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 244,
                column: "EndpointName",
                value: "Library.Publications.GetCountByPublishPlace");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 245,
                column: "EndpointName",
                value: "Library.Publications.GetCountBySubject");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 246,
                column: "EndpointName",
                value: "Library.Publications.GetCountByType");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 247,
                column: "EndpointName",
                value: "Library.Publications.GetList");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 248,
                column: "EndpointName",
                value: "Library.Publications.GetStatistics");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 249,
                column: "EndpointName",
                value: "Library.Publications.Search");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 250,
                column: "EndpointName",
                value: "Library.Publications.Update");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 251,
                column: "EndpointName",
                value: "Library.PublicationTypes.Create");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 252,
                column: "EndpointName",
                value: "Library.PublicationTypes.Delete");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 253,
                column: "EndpointName",
                value: "Library.PublicationTypes.GetAll");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 254,
                column: "EndpointName",
                value: "Library.PublicationTypes.GetById");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 255,
                column: "EndpointName",
                value: "Library.PublicationTypes.GetList");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 256,
                column: "EndpointName",
                value: "Library.PublicationTypes.GetNames");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 257,
                column: "EndpointName",
                value: "Library.PublicationTypes.Update");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 258,
                column: "EndpointName",
                value: "Library.Publishers.Create");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 259,
                column: "EndpointName",
                value: "Library.Publishers.Delete");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 260,
                column: "EndpointName",
                value: "Library.Publishers.GetAll");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 261,
                column: "EndpointName",
                value: "Library.Publishers.GetById");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 262,
                column: "EndpointName",
                value: "Library.Publishers.GetList");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 263,
                column: "EndpointName",
                value: "Library.Publishers.GetNames");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 264,
                column: "EndpointName",
                value: "Library.Publishers.Update");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 265,
                column: "EndpointName",
                value: "Library.Languages.Create");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 266,
                column: "EndpointName",
                value: "Library.Languages.Delete");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 267,
                column: "EndpointName",
                value: "Library.Languages.GetAll");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 268,
                column: "EndpointName",
                value: "Library.Languages.GetById");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 269,
                column: "EndpointName",
                value: "Library.Languages.GetList");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 270,
                column: "EndpointName",
                value: "Library.Languages.GetNames");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 271,
                column: "EndpointName",
                value: "Library.Languages.Update");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 272,
                column: "EndpointName",
                value: "Library.Cities.Create");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 273,
                column: "EndpointName",
                value: "Library.Cities.Delete");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 274,
                column: "EndpointName",
                value: "Library.Cities.GetAll");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 275,
                column: "EndpointName",
                value: "Library.Cities.GetById");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 276,
                column: "EndpointName",
                value: "Library.Cities.GetList");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 277,
                column: "EndpointName",
                value: "Library.Cities.GetNames");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 278,
                column: "EndpointName",
                value: "Library.Cities.Update");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 279,
                column: "EndpointName",
                value: "Library.Subjects.Create");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 280,
                column: "EndpointName",
                value: "Library.Subjects.Delete");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 281,
                column: "EndpointName",
                value: "Library.Subjects.GetAll");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 282,
                column: "EndpointName",
                value: "Library.Subjects.GetById");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 283,
                column: "EndpointName",
                value: "Library.Subjects.GetList");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 284,
                column: "EndpointName",
                value: "Library.Subjects.GetNames");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 285,
                column: "EndpointName",
                value: "Library.Subjects.Update");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 286,
                column: "EndpointName",
                value: "Library.Gaps.Create");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 287,
                column: "EndpointName",
                value: "Library.Gaps.Delete");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 288,
                column: "EndpointName",
                value: "Library.Gaps.GetAll");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 289,
                column: "EndpointName",
                value: "Library.Gaps.GetById");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 290,
                column: "EndpointName",
                value: "Library.Gaps.GetList");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 291,
                column: "EndpointName",
                value: "Library.Gaps.GetNames");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 292,
                column: "EndpointName",
                value: "Library.Gaps.Update");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 293,
                column: "EndpointName",
                value: "Library.Attachments.Upload");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 294,
                column: "EndpointName",
                value: "Library.Attachments.Delete");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 295,
                column: "EndpointName",
                value: "Library.Attachments.GetByEntity");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 296,
                columns: new[] { "EndpointName", "RoleId" },
                values: new object[] { "Library.Attachments.Update", 3 });
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 297,
                columns: new[] { "EndpointName", "RoleId" },
                values: new object[] { "Reports.BookSearchList", 3 });
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 298,
                columns: new[] { "EndpointName", "RoleId" },
                values: new object[] { "Reports.PublicationSearchList", 3 });
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 299,
                columns: new[] { "EndpointName", "RoleId" },
                values: new object[] { "Reports.ManuscriptSearchList", 3 });
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 300,
                columns: new[] { "EndpointName", "RoleId" },
                values: new object[] { "Reports.ManuscriptStatistics", 3 });
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 301,
                columns: new[] { "EndpointName", "RoleId" },
                values: new object[] { "Reports.BookStatistics", 3 });
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 302,
                columns: new[] { "EndpointName", "RoleId" },
                values: new object[] { "Reports.PublicationStatistics", 3 });
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 303,
                columns: new[] { "EndpointName", "RoleId" },
                values: new object[] { "Reports.Export", 3 });
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 304,
                columns: new[] { "EndpointName", "RoleId" },
                values: new object[] { "Reports.View", 3 });
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 305,
                column: "EndpointName",
                value: "Library.Settings.GetUserSettings");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 306,
                column: "EndpointName",
                value: "Library.Settings.UpdateUserSettings");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 307,
                column: "EndpointName",
                value: "Library.Settings.GetAppInfo");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 308,
                column: "EndpointName",
                value: "Library.Users.ChangePassword");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 309,
                column: "EndpointName",
                value: "Library.Users.UpdateProfile");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 310,
                column: "EndpointName",
                value: "Library.Menu.GetAll");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 311,
                column: "EndpointName",
                value: "Library.Visits.GetStats");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 312,
                column: "EndpointName",
                value: "Library.Publications.Create");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 313,
                column: "EndpointName",
                value: "Library.Publications.Delete");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 314,
                column: "EndpointName",
                value: "Library.Publications.GetAll");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 315,
                column: "EndpointName",
                value: "Library.Publications.GetAllEntities");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 316,
                column: "EndpointName",
                value: "Library.Publications.GetById");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 317,
                column: "EndpointName",
                value: "Library.Publications.GetCount");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 318,
                column: "EndpointName",
                value: "Library.Publications.GetCountByLanguage");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 319,
                column: "EndpointName",
                value: "Library.Publications.GetCountByPublishDate");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 320,
                column: "EndpointName",
                value: "Library.Publications.GetCountByPublishPlace");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 321,
                column: "EndpointName",
                value: "Library.Publications.GetCountBySubject");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 322,
                column: "EndpointName",
                value: "Library.Publications.GetCountByType");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 323,
                column: "EndpointName",
                value: "Library.Publications.GetList");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 324,
                column: "EndpointName",
                value: "Library.Publications.GetStatistics");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 325,
                column: "EndpointName",
                value: "Library.Publications.Search");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 326,
                column: "EndpointName",
                value: "Library.Publications.Update");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 327,
                column: "EndpointName",
                value: "Library.Publishers.Create");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 328,
                column: "EndpointName",
                value: "Library.Publishers.Delete");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 329,
                column: "EndpointName",
                value: "Library.Publishers.GetAll");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 330,
                column: "EndpointName",
                value: "Library.Publishers.GetById");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 331,
                column: "EndpointName",
                value: "Library.Publishers.GetList");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 332,
                column: "EndpointName",
                value: "Library.Publishers.GetNames");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 333,
                column: "EndpointName",
                value: "Library.Publishers.Update");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 334,
                column: "EndpointName",
                value: "Library.PublicationTypes.Create");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 335,
                column: "EndpointName",
                value: "Library.PublicationTypes.Delete");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 336,
                columns: new[] { "EndpointName", "RoleId" },
                values: new object[] { "Library.PublicationTypes.GetAll", 4 });
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 337,
                columns: new[] { "EndpointName", "RoleId" },
                values: new object[] { "Library.PublicationTypes.GetById", 4 });
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 338,
                columns: new[] { "EndpointName", "RoleId" },
                values: new object[] { "Library.PublicationTypes.GetList", 4 });
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 339,
                columns: new[] { "EndpointName", "RoleId" },
                values: new object[] { "Library.PublicationTypes.GetNames", 4 });
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 340,
                columns: new[] { "EndpointName", "RoleId" },
                values: new object[] { "Library.PublicationTypes.Update", 4 });
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 341,
                columns: new[] { "EndpointName", "RoleId" },
                values: new object[] { "Library.Attachments.Upload", 4 });
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 342,
                columns: new[] { "EndpointName", "RoleId" },
                values: new object[] { "Library.Attachments.Delete", 4 });
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 343,
                columns: new[] { "EndpointName", "RoleId" },
                values: new object[] { "Library.Attachments.GetByEntity", 4 });
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 344,
                columns: new[] { "EndpointName", "RoleId" },
                values: new object[] { "Library.Attachments.Update", 4 });
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 345,
                columns: new[] { "EndpointName", "RoleId" },
                values: new object[] { "Reports.BookSearchList", 4 });
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 346,
                columns: new[] { "EndpointName", "RoleId" },
                values: new object[] { "Reports.PublicationSearchList", 4 });
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 347,
                columns: new[] { "EndpointName", "RoleId" },
                values: new object[] { "Reports.ManuscriptSearchList", 4 });
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 348,
                columns: new[] { "EndpointName", "RoleId" },
                values: new object[] { "Reports.ManuscriptStatistics", 4 });
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 349,
                columns: new[] { "EndpointName", "RoleId" },
                values: new object[] { "Reports.BookStatistics", 4 });
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 350,
                columns: new[] { "EndpointName", "RoleId" },
                values: new object[] { "Reports.PublicationStatistics", 4 });
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 351,
                columns: new[] { "EndpointName", "RoleId" },
                values: new object[] { "Reports.Export", 4 });
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 352,
                columns: new[] { "EndpointName", "RoleId" },
                values: new object[] { "Reports.View", 4 });
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 353,
                column: "EndpointName",
                value: "Library.Settings.GetUserSettings");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 354,
                column: "EndpointName",
                value: "Library.Settings.UpdateUserSettings");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 355,
                column: "EndpointName",
                value: "Library.Settings.GetAppInfo");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 356,
                column: "EndpointName",
                value: "Library.Users.ChangePassword");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 357,
                column: "EndpointName",
                value: "Library.Users.UpdateProfile");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 358,
                column: "EndpointName",
                value: "Library.Menu.GetAll");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 359,
                column: "EndpointName",
                value: "Library.Visits.GetStats");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 360,
                column: "EndpointName",
                value: "Library.Books.GetAll");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 361,
                column: "EndpointName",
                value: "Library.Books.GetAllEntities");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 362,
                column: "EndpointName",
                value: "Library.Books.GetById");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 363,
                column: "EndpointName",
                value: "Library.Books.GetProfile");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 364,
                column: "EndpointName",
                value: "Library.Books.GetCount");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 365,
                column: "EndpointName",
                value: "Library.Books.GetCountByLanguage");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 366,
                column: "EndpointName",
                value: "Library.Books.GetCountByPublisher");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 367,
                column: "EndpointName",
                value: "Library.Books.GetCountByPublishPlace");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 368,
                column: "EndpointName",
                value: "Library.Books.GetCountBySubject");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 369,
                column: "EndpointName",
                value: "Library.Books.GetCountByYear");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 370,
                column: "EndpointName",
                value: "Library.Books.GetList");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 371,
                column: "EndpointName",
                value: "Library.Books.GetStatistics");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 372,
                column: "EndpointName",
                value: "Library.Books.GetVolumeCount");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 373,
                column: "EndpointName",
                value: "Library.Books.Search");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 374,
                column: "EndpointName",
                value: "Library.Manuscripts.GetAll");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 375,
                column: "EndpointName",
                value: "Library.Manuscripts.GetAllEntities");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 376,
                column: "EndpointName",
                value: "Library.Manuscripts.GetById");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 377,
                column: "EndpointName",
                value: "Library.Manuscripts.GetCount");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 378,
                column: "EndpointName",
                value: "Library.Manuscripts.GetCountByGap");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 379,
                column: "EndpointName",
                value: "Library.Manuscripts.GetCountByLanguage");
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: 380,
                column: "EndpointName",
                value: "Library.Manuscripts.GetCountBySubject");
            migrationBuilder.InsertData(
                schema: "dbo",
                table: "RolePermission",
                columns: new[] { "Id", "EndpointName", "RoleId" },
                values: new object[,]
                {
                    { 381, "Library.Manuscripts.GetCountByWritingPlace", 5 },
                    { 382, "Library.Manuscripts.GetCountByYear", 5 },
                    { 383, "Library.Manuscripts.GetList", 5 },
                    { 384, "Library.Manuscripts.GetStatistics", 5 },
                    { 385, "Library.Manuscripts.Search", 5 },
                    { 386, "Library.Publications.GetAll", 5 },
                    { 387, "Library.Publications.GetAllEntities", 5 },
                    { 388, "Library.Publications.GetById", 5 },
                    { 389, "Library.Publications.GetCount", 5 },
                    { 390, "Library.Publications.GetCountByLanguage", 5 },
                    { 391, "Library.Publications.GetCountByPublishDate", 5 },
                    { 392, "Library.Publications.GetCountByPublishPlace", 5 },
                    { 393, "Library.Publications.GetCountBySubject", 5 },
                    { 394, "Library.Publications.GetCountByType", 5 },
                    { 395, "Library.Publications.GetList", 5 },
                    { 396, "Library.Publications.GetStatistics", 5 },
                    { 397, "Library.Publications.Search", 5 },
                    { 398, "Reports.BookSearchList", 5 },
                    { 399, "Reports.PublicationSearchList", 5 },
                    { 400, "Reports.ManuscriptSearchList", 5 },
                    { 401, "Reports.ManuscriptStatistics", 5 },
                    { 402, "Reports.BookStatistics", 5 },
                    { 403, "Reports.PublicationStatistics", 5 },
                    { 404, "Reports.Export", 5 },
                    { 405, "Reports.View", 5 },
                    { 406, "Library.Attachments.GetByEntity", 5 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Attachment_EntityType_EntityId",
                schema: "dbo",
                table: "Attachment",
                columns: new[] { "EntityType", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_Book_Author",
                schema: "dbo",
                table: "Book",
                column: "Author");

            migrationBuilder.CreateIndex(
                name: "IX_Book_Isbn",
                schema: "dbo",
                table: "Book",
                column: "Isbn");

            migrationBuilder.CreateIndex(
                name: "IX_Book_LanguageId",
                schema: "dbo",
                table: "Book",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_Book_Name",
                schema: "dbo",
                table: "Book",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Book_No",
                schema: "dbo",
                table: "Book",
                column: "No");

            migrationBuilder.CreateIndex(
                name: "IX_Book_PublisherId",
                schema: "dbo",
                table: "Book",
                column: "PublisherId");

            migrationBuilder.CreateIndex(
                name: "IX_Book_PublishPlaceId",
                schema: "dbo",
                table: "Book",
                column: "PublishPlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_Book_PublishYear",
                schema: "dbo",
                table: "Book",
                column: "PublishYear");

            migrationBuilder.CreateIndex(
                name: "IX_Book_SubjectId",
                schema: "dbo",
                table: "Book",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_LibraryUsers_Email",
                schema: "dbo",
                table: "LibraryUsers",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_LibraryUsers_UserName",
                schema: "dbo",
                table: "LibraryUsers",
                column: "UserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Manuscript_Author",
                schema: "dbo",
                table: "Manuscript",
                column: "Author");

            migrationBuilder.CreateIndex(
                name: "IX_Manuscript_GapId",
                schema: "dbo",
                table: "Manuscript",
                column: "GapId");

            migrationBuilder.CreateIndex(
                name: "IX_Manuscript_LanguageId",
                schema: "dbo",
                table: "Manuscript",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_Manuscript_Name",
                schema: "dbo",
                table: "Manuscript",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Manuscript_SubjectId",
                schema: "dbo",
                table: "Manuscript",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Manuscript_VersionNo",
                schema: "dbo",
                table: "Manuscript",
                column: "VersionNo");

            migrationBuilder.CreateIndex(
                name: "IX_Manuscript_WritingPlaceId",
                schema: "dbo",
                table: "Manuscript",
                column: "WritingPlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_Manuscript_Year",
                schema: "dbo",
                table: "Manuscript",
                column: "Year");

            migrationBuilder.CreateIndex(
                name: "IX_MenuItem_DisplayOrder",
                schema: "dbo",
                table: "MenuItem",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_MenuItem_ParentId_DisplayOrder",
                schema: "dbo",
                table: "MenuItem",
                columns: new[] { "ParentId", "DisplayOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_Publication_JournalNo",
                schema: "dbo",
                table: "Publication",
                column: "JournalNo");

            migrationBuilder.CreateIndex(
                name: "IX_Publication_LanguageId",
                schema: "dbo",
                table: "Publication",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_Publication_Name",
                schema: "dbo",
                table: "Publication",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Publication_No",
                schema: "dbo",
                table: "Publication",
                column: "No");

            migrationBuilder.CreateIndex(
                name: "IX_Publication_PublishPlaceId",
                schema: "dbo",
                table: "Publication",
                column: "PublishPlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_Publication_SubjectId",
                schema: "dbo",
                table: "Publication",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Publication_TypeId",
                schema: "dbo",
                table: "Publication",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Publication_Year",
                schema: "dbo",
                table: "Publication",
                column: "Year");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermission_RoleId_EndpointName",
                schema: "dbo",
                table: "RolePermission",
                columns: new[] { "RoleId", "EndpointName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_UserId_RoleId",
                schema: "dbo",
                table: "UserRole",
                columns: new[] { "UserId", "RoleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSettings_ProfileImageAttachmentId",
                schema: "dbo",
                table: "UserSettings",
                column: "ProfileImageAttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSettings_UserId",
                schema: "dbo",
                table: "UserSettings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitLog_VisitedAt",
                schema: "dbo",
                table: "VisitLog",
                column: "VisitedAt");

            migrationBuilder.Sql(
                @"EXEC(N'CREATE OR ALTER PROCEDURE [dbo].[usp_BackupDatabase]
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
            END')");

            migrationBuilder.Sql(
                @"EXEC(N'CREATE OR ALTER PROCEDURE dbo.usp_MaintainDatabase
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

    -- ۱) بازسازی ایندکسهای پرfragmentation (فقط > 20% و حداقل 1000 صفحه)
    --    این کار هم جستجوها را تند میکند هم فضای آزاد میسازد که shrink آن را میگیرد
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
            -- اگر shrink ممکن نبود (لاگ در حال استفاده) بیخیال
        END CATCH
    END

    -- ۳) shrink دیتا فقط اگر فضای آزاد واقعی داشته باشیم (DBCC SHRINKFILE فایل را کوچک میکند نه محتوا را)
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
    --    نام فایل باید ثابت باشد (ALTER DATABASE با متغیر پشتیبانی نمیشود)
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
END')");
        }

        
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppInfo",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Book",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Manuscript",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "MenuItem",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Publication",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "RefreshTokens",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "RolePermission",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Settings",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "UserRole",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "UserSettings",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "VisitLog",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Publisher",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Gap",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "City",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Language",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PublicationType",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Subject",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Role",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Attachment",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "LibraryUsers",
                schema: "dbo");
        }
    }
}
