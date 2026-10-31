using System.Collections.Immutable;

namespace Application.Library.Settings;

public static class EndpointCatalog
{
    public static readonly ImmutableArray<string> Books = ImmutableArray.Create(
        "Library.Books.Create",
        "Library.Books.Delete",
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
        "Library.Books.Search",
        "Library.Books.Update");

    public static readonly ImmutableArray<string> Cities = ImmutableArray.Create(
        "Library.Cities.Create",
        "Library.Cities.Delete",
        "Library.Cities.GetAll",
        "Library.Cities.GetById",
        "Library.Cities.GetList",
        "Library.Cities.GetNames",
        "Library.Cities.Update");

    public static readonly ImmutableArray<string> Gaps = ImmutableArray.Create(
        "Library.Gaps.Create",
        "Library.Gaps.Delete",
        "Library.Gaps.GetAll",
        "Library.Gaps.GetById",
        "Library.Gaps.GetList",
        "Library.Gaps.GetNames",
        "Library.Gaps.Update");

    public static readonly ImmutableArray<string> Languages = ImmutableArray.Create(
        "Library.Languages.Create",
        "Library.Languages.Delete",
        "Library.Languages.GetAll",
        "Library.Languages.GetById",
        "Library.Languages.GetList",
        "Library.Languages.GetNames",
        "Library.Languages.Update");

    public static readonly ImmutableArray<string> Manuscripts = ImmutableArray.Create(
        "Library.Manuscripts.Create",
        "Library.Manuscripts.Delete",
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
        "Library.Manuscripts.Search",
        "Library.Manuscripts.Update");

    public static readonly ImmutableArray<string> Publications = ImmutableArray.Create(
        "Library.Publications.Create",
        "Library.Publications.Delete",
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
        "Library.Publications.Search",
        "Library.Publications.Update");

    public static readonly ImmutableArray<string> PublicationTypes = ImmutableArray.Create(
        "Library.PublicationTypes.Create",
        "Library.PublicationTypes.Delete",
        "Library.PublicationTypes.GetAll",
        "Library.PublicationTypes.GetById",
        "Library.PublicationTypes.GetList",
        "Library.PublicationTypes.GetNames",
        "Library.PublicationTypes.Update");

    public static readonly ImmutableArray<string> Publishers = ImmutableArray.Create(
        "Library.Publishers.Create",
        "Library.Publishers.Delete",
        "Library.Publishers.GetAll",
        "Library.Publishers.GetById",
        "Library.Publishers.GetList",
        "Library.Publishers.GetNames",
        "Library.Publishers.Update");

    public static readonly ImmutableArray<string> Settings = ImmutableArray.Create(
        "Library.Settings.AssignRoleToUser",
        "Library.Settings.CreateRole",
        "Library.Settings.GetAll",
        "Library.Settings.GetAllEntities",
        "Library.Settings.GetAllEndpoints",
        "Library.Settings.GetAllRoles",
        "Library.Settings.GetAppInfo",
        "Library.Settings.GetBackground",
        "Library.Settings.GetById",
        "Library.Settings.GetByName",
        "Library.Settings.GetMainTitle",
        "Library.Settings.GetRolePermissions",
        "Library.Settings.GetSystemInfo",
        "Library.Settings.GetUserRoles",
        "Library.Settings.GetUserSettings",
        "Library.Settings.RemoveRoleFromUser",
        "Library.Settings.Update",
        "Library.Settings.UpdateRoleEndpoints",
        "Library.Settings.UpdateUserSettings");

    public static readonly ImmutableArray<string> Subjects = ImmutableArray.Create(
        "Library.Subjects.Create",
        "Library.Subjects.Delete",
        "Library.Subjects.GetAll",
        "Library.Subjects.GetById",
        "Library.Subjects.GetList",
        "Library.Subjects.GetNames",
        "Library.Subjects.Update");

    public static readonly ImmutableArray<string> Users = ImmutableArray.Create(
        "Library.Users.ChangePassword",
        "Library.Users.Create",
        "Library.Users.Delete",
        "Library.Users.GetAll",
        "Library.Users.GetAllEntities",
        "Library.Users.GetByUserName",
        "Library.Users.GetFullName",
        "Library.Users.GetList",
        "Library.Users.Revoke",
        "Library.Users.Update",
        "Library.Users.UpdateProfile");

    public static readonly ImmutableArray<string> Attachments = ImmutableArray.Create(
        "Library.Attachments.Upload",
        "Library.Attachments.Delete",
        "Library.Attachments.GetByEntity",
        "Library.Attachments.Update");

    public static readonly ImmutableArray<string> Menu = ImmutableArray.Create(
        "Library.Menu.GetAll",
        "Library.Menu.Create",
        "Library.Menu.Update",
        "Library.Menu.Delete",
        "Library.Menu.Reorder",
        "Library.Menu.ResetToDefault");

    public static readonly ImmutableArray<string> Visits = ImmutableArray.Create(
        "Library.Visits.GetStats");

    public static readonly ImmutableArray<string> Backup = ImmutableArray.Create(
        "Library.Backup.GetInfo",
        "Library.Backup.Create",
        "Library.Backup.Download",
        "Library.Backup.Restore");

    public static readonly ImmutableArray<string> Reports = ImmutableArray.Create(
        "Reports.BookSearchList",
        "Reports.PublicationSearchList",
        "Reports.ManuscriptSearchList",
        "Reports.ManuscriptStatistics",
        "Reports.BookStatistics",
        "Reports.PublicationStatistics",
        "Reports.Export",
        "Reports.View");

    private static readonly ImmutableArray<string> Combined = ImmutableArray.CreateRange(
        Books
            .Concat(Cities)
            .Concat(Gaps)
            .Concat(Languages)
            .Concat(Manuscripts)
            .Concat(Publications)
            .Concat(PublicationTypes)
            .Concat(Publishers)
            .Concat(Settings)
            .Concat(Subjects)
            .Concat(Users)
            .Concat(Attachments)
            .Concat(Menu)
            .Concat(Visits)
            .Concat(Backup)
            .Concat(Reports));

    public static readonly ImmutableHashSet<string> All = Combined.ToImmutableHashSet(StringComparer.OrdinalIgnoreCase);

    public static bool IsKnownEndpoint(string endpointName) =>
        !string.IsNullOrWhiteSpace(endpointName) && All.Contains(endpointName);

    
    
    
    public static Dictionary<string, List<string>> GetAllEndpointsGrouped()
    {
        return new Dictionary<string, List<string>>
        {
            { "Books", Books.ToList() },
            { "Cities", Cities.ToList() },
            { "Gaps", Gaps.ToList() },
            { "Languages", Languages.ToList() },
            { "Manuscripts", Manuscripts.ToList() },
            { "Publications", Publications.ToList() },
            { "PublicationTypes", PublicationTypes.ToList() },
            { "Publishers", Publishers.ToList() },
            { "Settings", Settings.ToList() },
            { "Subjects", Subjects.ToList() },
            { "Users", Users.ToList() },
            { "Attachments", Attachments.ToList() },
            { "Menu", Menu.ToList() },
            { "Reports", Reports.ToList() }
        };
    }

    
    
    
    public static List<string> GetAllEndpoints() => Combined.ToList();
}

