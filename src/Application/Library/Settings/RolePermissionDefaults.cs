using System.Collections.Immutable;

namespace Application.Library.Settings;

public static class RolePermissionDefaults
{
    public const string Admin = "Admin";
    public const string User = "User";
    public const string Librarian = "Librarian";
    public const string Publisher = "Publisher";
    public const string Author = "Author";

    
    public static readonly ImmutableArray<string> SelfService = ImmutableArray.Create(
        "Library.Settings.GetUserSettings",
        "Library.Settings.UpdateUserSettings",
        "Library.Settings.GetAppInfo",
        "Library.Users.ChangePassword",
        "Library.Users.UpdateProfile",
        "Library.Menu.GetAll");

    public static readonly ImmutableArray<string> DashboardRead = ImmutableArray.Create(
        "Library.Visits.GetStats");

    
    private static readonly ImmutableArray<string> ReportsReadOnly = ImmutableArray.Create(
        "Reports.BookSearchList",
        "Reports.PublicationSearchList",
        "Reports.ManuscriptSearchList",
        "Reports.ManuscriptStatistics",
        "Reports.BookStatistics",
        "Reports.PublicationStatistics",
        "Reports.Export",
        "Reports.View");

    
    private static readonly ImmutableArray<string> AttachmentView = ImmutableArray.Create(
        "Library.Attachments.GetByEntity");

    public static readonly ImmutableArray<string> LibrarianEndpoints =
        ImmutableArray.CreateRange(
            SelfService
                .Concat(DashboardRead)
                .Concat(EndpointCatalog.Books)
                .Concat(EndpointCatalog.Manuscripts)
                .Concat(EndpointCatalog.Publications)
                .Concat(EndpointCatalog.PublicationTypes)
                .Concat(EndpointCatalog.Publishers)
                .Concat(EndpointCatalog.Languages)
                .Concat(EndpointCatalog.Cities)
                .Concat(EndpointCatalog.Subjects)
                .Concat(EndpointCatalog.Gaps)
                .Concat(EndpointCatalog.Attachments)
                .Concat(EndpointCatalog.Reports));

    public static readonly ImmutableArray<string> PublisherEndpoints =
        ImmutableArray.CreateRange(
            SelfService
                .Concat(DashboardRead)
                .Concat(EndpointCatalog.Publications)
                .Concat(EndpointCatalog.Publishers)
                .Concat(EndpointCatalog.PublicationTypes)
                .Concat(EndpointCatalog.Attachments)
                .Concat(ReportsReadOnly));

    private static readonly ImmutableArray<string> BooksReadOnly = ImmutableArray.Create(
        "Library.Books.GetAll",
        "Library.Books.GetAllEntities",
        "Library.Books.GetById",
        "Library.Books.GetProfile",
        "Library.Books.GetCount",
        "Library.Books.GetCountByLanguage",
        "Library.Books.GetCountByPublisher",
        "Library.Books.GetCountByPublishPlace",
        "Library.Books.GetCountBySubject",
        "Library.Books.GetCountByYear",
        "Library.Books.GetList",
        "Library.Books.GetStatistics",
        "Library.Books.GetVolumeCount",
        "Library.Books.Search");

    private static readonly ImmutableArray<string> ManuscriptsReadOnly = ImmutableArray.Create(
        "Library.Manuscripts.GetAll",
        "Library.Manuscripts.GetAllEntities",
        "Library.Manuscripts.GetById",
        "Library.Manuscripts.GetCount",
        "Library.Manuscripts.GetCountByGap",
        "Library.Manuscripts.GetCountByLanguage",
        "Library.Manuscripts.GetCountBySubject",
        "Library.Manuscripts.GetCountByWritingPlace",
        "Library.Manuscripts.GetCountByYear",
        "Library.Manuscripts.GetList",
        "Library.Manuscripts.GetStatistics",
        "Library.Manuscripts.Search");

    private static readonly ImmutableArray<string> PublicationsReadOnly = ImmutableArray.Create(
        "Library.Publications.GetAll",
        "Library.Publications.GetAllEntities",
        "Library.Publications.GetById",
        "Library.Publications.GetCount",
        "Library.Publications.GetCountByLanguage",
        "Library.Publications.GetCountByPublishDate",
        "Library.Publications.GetCountByPublishPlace",
        "Library.Publications.GetCountBySubject",
        "Library.Publications.GetCountByType",
        "Library.Publications.GetList",
        "Library.Publications.GetStatistics",
        "Library.Publications.Search");

    
    public static readonly ImmutableArray<string> UserEndpoints =
        ImmutableArray.CreateRange(
            SelfService
                .Concat(DashboardRead)
                .Concat(BooksReadOnly)
                .Concat(ManuscriptsReadOnly)
                .Concat(PublicationsReadOnly)
                .Concat(ReportsReadOnly)
                .Concat(AttachmentView));

    public static readonly ImmutableArray<string> AuthorEndpoints =
        ImmutableArray.CreateRange(
            SelfService
                .Concat(DashboardRead)
                .Concat(BooksReadOnly)
                .Concat(ManuscriptsReadOnly)
                .Concat(PublicationsReadOnly)
                .Concat(ReportsReadOnly)
                .Concat(AttachmentView));

    public static readonly ImmutableDictionary<string, ImmutableArray<string>> RoleEndpointMapping =
        new Dictionary<string, ImmutableArray<string>>(StringComparer.OrdinalIgnoreCase)
        {
            [User] = UserEndpoints,
            [Librarian] = LibrarianEndpoints,
            [Publisher] = PublisherEndpoints,
            [Author] = AuthorEndpoints
        }.ToImmutableDictionary(pair => pair.Key, pair => pair.Value);

    public static bool TryGetDefaults(string roleName, out ImmutableArray<string> endpoints) =>
        RoleEndpointMapping.TryGetValue(roleName, out endpoints);
}

