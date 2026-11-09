using Application.Abstractions.Data;
using Domain.Library;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public sealed class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
        
        
        Database.SetCommandTimeout(TimeSpan.FromMinutes(5));
    }

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
    public DbSet<RoleEndpoint> RoleEndpoints { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Attachment> Attachments { get; set; }
    public DbSet<UserSettings> UserSettings { get; set; }
    public DbSet<AppInfo> AppInfos { get; set; }
    public DbSet<MenuItem> MenuItems { get; set; }
    public DbSet<VisitLog> VisitLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<TEntity> GetDbSet<TEntity>() where TEntity : class
    {
        return Set<TEntity>();
    }
}
