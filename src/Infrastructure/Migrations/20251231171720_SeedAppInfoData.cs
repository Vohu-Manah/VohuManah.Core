using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    
    public partial class SeedAppInfoData : Migration
    {
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AppInfo]') AND type in (N'U'))
                AND NOT EXISTS (SELECT 1 FROM [dbo].[AppInfo])
                BEGIN
                    SET IDENTITY_INSERT [dbo].[AppInfo] ON;
                    INSERT INTO [dbo].[AppInfo] ([Id], [AppName], [Version], [Author], [Description], [LastUpdateDate], [Copyright], [License], [Website], [SupportEmail]) VALUES
                        (1, N'Vohu Manah', N'1.0.0', N'Vohu Manah Team', N'سیستم مدیریت کتابخانه', GETUTCDATE(), N'© 2025 Vohu Manah', N'Proprietary', N'https://vohumanah.com', N'support@vohumanah.com');
                    SET IDENTITY_INSERT [dbo].[AppInfo] OFF;
                END
            ");

        }

        
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AppInfo]') AND type in (N'U'))
                BEGIN
                    DELETE FROM [dbo].[AppInfo] WHERE [Id] = 1;
                END
            ");
        }
    }
}
