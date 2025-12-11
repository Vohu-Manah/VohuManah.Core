using Application.Abstractions.Data;
using Domain.Library;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public sealed class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options)
    : DbContext(options), IApplicationDbContext
{
    public DbSet<Domain.Library.User> LibraryUsers { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<Language> Languages { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<Publisher> Publishers { get; set; }
    public DbSet<Publication> Publications { get; set; }
    public DbSet<PublicationType> PublicationTypes { get; set; }
    public DbSet<Manuscript> Manuscripts { get; set; }
    public DbSet<Gap> Gaps { get; set; }
    public DbSet<Settings> Settings { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    public new DbSet<TEntity> Set<TEntity>() where TEntity : class
    {
        return base.Set<TEntity>();
    }

    int IUnitOfWork.SaveChanges()
    {
        return SaveChanges();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        modelBuilder.HasDefaultSchema(Schemas.Default);
        
        Infrastructure.Library.LibrarySeedData.SeedData(modelBuilder);
    }
}
