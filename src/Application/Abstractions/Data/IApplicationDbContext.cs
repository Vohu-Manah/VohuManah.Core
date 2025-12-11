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
            DbSet<RefreshToken> RefreshTokens { get; }
}

public interface IUnitOfWork
{
    DbSet<TEntity> Set<TEntity>() where TEntity : class;
    int SaveChanges();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
