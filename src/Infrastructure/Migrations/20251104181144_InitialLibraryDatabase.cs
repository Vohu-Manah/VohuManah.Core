using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional
#pragma warning disable CA1861 // Prefer 'static readonly' fields over constant array arguments

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialLibraryDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

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
                    UserName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LibraryUsers", x => x.UserName);
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
                name: "Users",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
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

            migrationBuilder.CreateTable(
                name: "BaseEntityItems",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Labels = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseEntityItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BaseEntityItems_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                columns: new[] { "UserName", "LastName", "Name", "Password" },
                values: new object[] { "admin", "admin", "admin", "123456" });

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

            migrationBuilder.CreateIndex(
                name: "IX_BaseEntityItems_UserId",
                schema: "dbo",
                table: "BaseEntityItems",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Book_LanguageId",
                schema: "dbo",
                table: "Book",
                column: "LanguageId");

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
                name: "IX_Book_SubjectId",
                schema: "dbo",
                table: "Book",
                column: "SubjectId");

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
                name: "IX_Manuscript_SubjectId",
                schema: "dbo",
                table: "Manuscript",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Manuscript_WritingPlaceId",
                schema: "dbo",
                table: "Manuscript",
                column: "WritingPlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_Publication_LanguageId",
                schema: "dbo",
                table: "Publication",
                column: "LanguageId");

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
                name: "IX_Users_Email",
                schema: "dbo",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BaseEntityItems",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Book",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "LibraryUsers",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Manuscript",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Publication",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Settings",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Users",
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
        }
    }
}
