using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedRolePermissionsForAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Seed RolePermission data for Admin role
            migrationBuilder.Sql(@"
                DECLARE @AdminRoleId INT = (SELECT Id FROM [dbo].[Role] WHERE [Name] = 'Admin');
                
                IF @AdminRoleId IS NOT NULL
                BEGIN
                    MERGE [dbo].[RolePermission] AS target
                    USING (VALUES
                        ('Library.Books.Create'), ('Library.Books.Update'), ('Library.Books.Delete'),
                        ('Library.Books.GetById'), ('Library.Books.GetAll'), ('Library.Books.GetList'),
                        ('Library.Books.GetAllEntities'), ('Library.Books.GetCount'),
                        ('Library.Books.GetCountByLanguage'), ('Library.Books.GetCountByPublisher'),
                        ('Library.Books.GetCountByPublishPlace'), ('Library.Books.GetCountBySubject'),
                        ('Library.Books.GetCountByYear'), ('Library.Books.GetStatistics'),
                        ('Library.Books.GetVolumeCount'), ('Library.Books.Search'),
                        ('Library.Cities.Create'), ('Library.Cities.Update'), ('Library.Cities.Delete'),
                        ('Library.Cities.GetById'), ('Library.Cities.GetAll'), ('Library.Cities.GetList'),
                        ('Library.Cities.GetNames'),
                        ('Library.Gaps.Create'), ('Library.Gaps.Update'), ('Library.Gaps.Delete'),
                        ('Library.Gaps.GetById'), ('Library.Gaps.GetAll'), ('Library.Gaps.GetList'),
                        ('Library.Gaps.GetNames'),
                        ('Library.Languages.Create'), ('Library.Languages.Update'), ('Library.Languages.Delete'),
                        ('Library.Languages.GetById'), ('Library.Languages.GetAll'), ('Library.Languages.GetList'),
                        ('Library.Languages.GetNames'),
                        ('Library.Manuscripts.Create'), ('Library.Manuscripts.Update'), ('Library.Manuscripts.Delete'),
                        ('Library.Manuscripts.GetById'), ('Library.Manuscripts.GetAll'), ('Library.Manuscripts.GetList'),
                        ('Library.Manuscripts.GetAllEntities'), ('Library.Manuscripts.GetCount'),
                        ('Library.Manuscripts.GetCountByGap'), ('Library.Manuscripts.GetCountByLanguage'),
                        ('Library.Manuscripts.GetCountBySubject'), ('Library.Manuscripts.GetCountByWritingPlace'),
                        ('Library.Manuscripts.GetCountByYear'), ('Library.Manuscripts.GetStatistics'),
                        ('Library.Manuscripts.Search'),
                        ('Library.Publications.Create'), ('Library.Publications.Update'), ('Library.Publications.Delete'),
                        ('Library.Publications.GetById'), ('Library.Publications.GetAll'), ('Library.Publications.GetList'),
                        ('Library.Publications.GetAllEntities'), ('Library.Publications.GetCount'),
                        ('Library.Publications.GetCountByLanguage'), ('Library.Publications.GetCountByPublishDate'),
                        ('Library.Publications.GetCountByPublishPlace'), ('Library.Publications.GetCountBySubject'),
                        ('Library.Publications.GetCountByType'), ('Library.Publications.GetStatistics'),
                        ('Library.Publications.Search'),
                        ('Library.PublicationTypes.Create'), ('Library.PublicationTypes.Update'), ('Library.PublicationTypes.Delete'),
                        ('Library.PublicationTypes.GetById'), ('Library.PublicationTypes.GetAll'), ('Library.PublicationTypes.GetList'),
                        ('Library.PublicationTypes.GetNames'),
                        ('Library.Publishers.Create'), ('Library.Publishers.Update'), ('Library.Publishers.Delete'),
                        ('Library.Publishers.GetById'), ('Library.Publishers.GetAll'), ('Library.Publishers.GetList'),
                        ('Library.Publishers.GetNames'),
                        ('Library.Settings.Update'), ('Library.Settings.GetById'), ('Library.Settings.GetAll'),
                        ('Library.Settings.GetAllEntities'), ('Library.Settings.GetByName'),
                        ('Library.Settings.GetBackground'), ('Library.Settings.GetMainTitle'),
                        ('Library.Settings.GetAllRoles'), ('Library.Settings.GetUserRoles'),
                        ('Library.Settings.AssignRoleToUser'), ('Library.Settings.RemoveRoleFromUser'),
                        ('Library.Subjects.Create'), ('Library.Subjects.Update'), ('Library.Subjects.Delete'),
                        ('Library.Subjects.GetById'), ('Library.Subjects.GetAll'), ('Library.Subjects.GetList'),
                        ('Library.Subjects.GetNames'),
                        ('Library.Users.Create'), ('Library.Users.Update'), ('Library.Users.Delete'),
                        ('Library.Users.GetAll'), ('Library.Users.GetList'), ('Library.Users.GetAllEntities'),
                        ('Library.Users.GetByUserName'), ('Library.Users.GetFullName'), ('Library.Users.Revoke')
                    ) AS source (EndpointName)
                    ON target.RoleId = @AdminRoleId AND target.EndpointName = source.EndpointName
                    WHEN NOT MATCHED THEN
                        INSERT (RoleId, EndpointName)
                        VALUES (@AdminRoleId, source.EndpointName);
                END
            ");
            
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "LibraryUsers",
                keyColumn: "Id",
                keyValue: 1L,
                column: "Password",
                value: "648E9E14DB5F5EBC7B5E3892EE6E5B4C7C49A233DBDAC3C169C33708CC95EF44-65DF715A949CBD1C8FE829424B5FD78E");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove RolePermission data for Admin role
            migrationBuilder.Sql(@"
                DECLARE @AdminRoleId INT = (SELECT Id FROM [dbo].[Role] WHERE [Name] = 'Admin');
                
                IF @AdminRoleId IS NOT NULL
                BEGIN
                    DELETE FROM [dbo].[RolePermission] WHERE RoleId = @AdminRoleId;
                END
            ");
            
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "LibraryUsers",
                keyColumn: "Id",
                keyValue: 1L,
                column: "Password",
                value: "F5DBDB150DB276F90D7B9660461ABA07D1B8AB0364242AD52FEAA88B3680DAC3-BB19FC50EAE2BA0F64974B3911FBD6EB");
        }
    }
}
