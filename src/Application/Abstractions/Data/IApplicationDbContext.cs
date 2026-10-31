using Domain.Library;
using Microsoft.EntityFrameworkCore;

namespace Application.Abstractions.Data;

public interface IApplicationDbContext : IUnitOfWork
{
    DbSet<Domain.Library.User> LibraryUsers { get; }
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
    DbSet<RoleEndpoint> RoleEndpoints { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<Attachment> Attachments { get; }
    DbSet<UserSettings> UserSettings { get; }
    DbSet<AppInfo> AppInfos { get; }
    DbSet<MenuItem> MenuItems { get; }
    DbSet<VisitLog> VisitLogs { get; }
}

public interface IUnitOfWork
{
    DbSet<TEntity> GetDbSet<TEntity>() where TEntity : class;
    int SaveChanges();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
