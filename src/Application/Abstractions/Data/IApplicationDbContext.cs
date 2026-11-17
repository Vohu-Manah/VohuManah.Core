using VohuManah.Domain.AppSettings;
using VohuManah.Domain.Attachments;
using VohuManah.Domain.Books;
using VohuManah.Domain.Cities;
using VohuManah.Domain.Gaps;
using VohuManah.Domain.Languages;
using VohuManah.Domain.Manuscripts;
using VohuManah.Domain.Menu;
using VohuManah.Domain.PublicationTypes;
using VohuManah.Domain.Publications;
using VohuManah.Domain.Publishers;
using VohuManah.Domain.Subjects;
using VohuManah.Domain.Users;
using VohuManah.Domain.Visits;
using Microsoft.EntityFrameworkCore;

namespace VohuManah.Application.Abstractions.Data;

public interface IApplicationDbContext
{
    DbSet<VohuManah.Domain.Users.User> LibraryUsers { get; }
    DbSet<Book> Books { get; }
    DbSet<City> Cities { get; }
    DbSet<Language> Languages { get; }
    DbSet<Subject> Subjects { get; }
    DbSet<Publisher> Publishers { get; }
    DbSet<Publication> Publications { get; }
    DbSet<PublicationType> PublicationTypes { get; }
    DbSet<Manuscript> Manuscripts { get; }
    DbSet<Gap> Gaps { get; }
    DbSet<Settings> Settings { get; }
    DbSet<Role> Roles { get; }
    DbSet<UserRole> UserRoles { get; }
    DbSet<RolePermission> RolePermissions { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<Attachment> Attachments { get; }
    DbSet<UserSettings> UserSettings { get; }
    DbSet<AppInfo> AppInfos { get; }
    DbSet<MenuItem> MenuItems { get; }
    DbSet<VisitLog> VisitLogs { get; }
    int SaveChanges();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
