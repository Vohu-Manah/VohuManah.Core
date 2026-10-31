namespace Application.Library.Settings;

#pragma warning disable CA1034
public static class EndpointNames
{
    public static class Books
    {
        public const string Create = "Library.Books.Create";
        public const string Delete = "Library.Books.Delete";
        public const string GetAll = "Library.Books.GetAll";
        public const string GetAllEntities = "Library.Books.GetAllEntities";
        public const string GetById = "Library.Books.GetById";
        public const string GetProfile = "Library.Books.GetProfile";
        public const string GetCount = "Library.Books.GetCount";
        public const string GetCountByLanguage = "Library.Books.GetCountByLanguage";
        public const string GetCountByPublisher = "Library.Books.GetCountByPublisher";
        public const string GetCountByPublishPlace = "Library.Books.GetCountByPublishPlace";
        public const string GetCountBySubject = "Library.Books.GetCountBySubject";
        public const string GetCountByYear = "Library.Books.GetCountByYear";
        public const string GetList = "Library.Books.GetList";
        public const string GetStatistics = "Library.Books.GetStatistics";
        public const string GetVolumeCount = "Library.Books.GetVolumeCount";
        public const string Search = "Library.Books.Search";
        public const string Update = "Library.Books.Update";
    }

    public static class Cities
    {
        public const string Create = "Library.Cities.Create";
        public const string Delete = "Library.Cities.Delete";
        public const string GetAll = "Library.Cities.GetAll";
        public const string GetById = "Library.Cities.GetById";
        public const string GetList = "Library.Cities.GetList";
        public const string GetNames = "Library.Cities.GetNames";
        public const string Update = "Library.Cities.Update";
    }

    public static class Gaps
    {
        public const string Create = "Library.Gaps.Create";
        public const string Delete = "Library.Gaps.Delete";
        public const string GetAll = "Library.Gaps.GetAll";
        public const string GetById = "Library.Gaps.GetById";
        public const string GetList = "Library.Gaps.GetList";
        public const string GetNames = "Library.Gaps.GetNames";
        public const string Update = "Library.Gaps.Update";
    }

    public static class Languages
    {
        public const string Create = "Library.Languages.Create";
        public const string Delete = "Library.Languages.Delete";
        public const string GetAll = "Library.Languages.GetAll";
        public const string GetById = "Library.Languages.GetById";
        public const string GetList = "Library.Languages.GetList";
        public const string GetNames = "Library.Languages.GetNames";
        public const string Update = "Library.Languages.Update";
    }

    public static class Manuscripts
    {
        public const string Create = "Library.Manuscripts.Create";
        public const string Delete = "Library.Manuscripts.Delete";
        public const string GetAll = "Library.Manuscripts.GetAll";
        public const string GetAllEntities = "Library.Manuscripts.GetAllEntities";
        public const string GetById = "Library.Manuscripts.GetById";
        public const string GetCount = "Library.Manuscripts.GetCount";
        public const string GetCountByGap = "Library.Manuscripts.GetCountByGap";
        public const string GetCountByLanguage = "Library.Manuscripts.GetCountByLanguage";
        public const string GetCountBySubject = "Library.Manuscripts.GetCountBySubject";
        public const string GetCountByWritingPlace = "Library.Manuscripts.GetCountByWritingPlace";
        public const string GetCountByYear = "Library.Manuscripts.GetCountByYear";
        public const string GetList = "Library.Manuscripts.GetList";
        public const string GetStatistics = "Library.Manuscripts.GetStatistics";
        public const string Search = "Library.Manuscripts.Search";
        public const string Update = "Library.Manuscripts.Update";
    }

    public static class Publications
    {
        public const string Create = "Library.Publications.Create";
        public const string Delete = "Library.Publications.Delete";
        public const string GetAll = "Library.Publications.GetAll";
        public const string GetAllEntities = "Library.Publications.GetAllEntities";
        public const string GetById = "Library.Publications.GetById";
        public const string GetCount = "Library.Publications.GetCount";
        public const string GetCountByLanguage = "Library.Publications.GetCountByLanguage";
        public const string GetCountByPublishDate = "Library.Publications.GetCountByPublishDate";
        public const string GetCountByPublishPlace = "Library.Publications.GetCountByPublishPlace";
        public const string GetCountBySubject = "Library.Publications.GetCountBySubject";
        public const string GetCountByType = "Library.Publications.GetCountByType";
        public const string GetList = "Library.Publications.GetList";
        public const string GetStatistics = "Library.Publications.GetStatistics";
        public const string Search = "Library.Publications.Search";
        public const string Update = "Library.Publications.Update";
    }

    public static class PublicationTypes
    {
        public const string Create = "Library.PublicationTypes.Create";
        public const string Delete = "Library.PublicationTypes.Delete";
        public const string GetAll = "Library.PublicationTypes.GetAll";
        public const string GetById = "Library.PublicationTypes.GetById";
        public const string GetList = "Library.PublicationTypes.GetList";
        public const string GetNames = "Library.PublicationTypes.GetNames";
        public const string Update = "Library.PublicationTypes.Update";
    }

    public static class Publishers
    {
        public const string Create = "Library.Publishers.Create";
        public const string Delete = "Library.Publishers.Delete";
        public const string GetAll = "Library.Publishers.GetAll";
        public const string GetById = "Library.Publishers.GetById";
        public const string GetList = "Library.Publishers.GetList";
        public const string GetNames = "Library.Publishers.GetNames";
        public const string Update = "Library.Publishers.Update";
    }

    public static class Settings
    {
        public const string AssignRoleToUser = "Library.Settings.AssignRoleToUser";
        public const string CreateRole = "Library.Settings.CreateRole";
        public const string GetAll = "Library.Settings.GetAll";
        public const string GetAllEntities = "Library.Settings.GetAllEntities";
        public const string GetAllEndpoints = "Library.Settings.GetAllEndpoints";
        public const string GetAllRoles = "Library.Settings.GetAllRoles";
        public const string GetAppInfo = "Library.Settings.GetAppInfo";
        public const string GetBackground = "Library.Settings.GetBackground";
        public const string GetById = "Library.Settings.GetById";
        public const string GetByName = "Library.Settings.GetByName";
        public const string GetMainTitle = "Library.Settings.GetMainTitle";
        public const string GetRolePermissions = "Library.Settings.GetRolePermissions";
        public const string GetSystemInfo = "Library.Settings.GetSystemInfo";
        public const string GetUserRoles = "Library.Settings.GetUserRoles";
        public const string GetUserSettings = "Library.Settings.GetUserSettings";
        public const string RemoveRoleFromUser = "Library.Settings.RemoveRoleFromUser";
        public const string Update = "Library.Settings.Update";
        public const string UpdateRoleEndpoints = "Library.Settings.UpdateRoleEndpoints";
        public const string UpdateUserSettings = "Library.Settings.UpdateUserSettings";
    }

    public static class Subjects
    {
        public const string Create = "Library.Subjects.Create";
        public const string Delete = "Library.Subjects.Delete";
        public const string GetAll = "Library.Subjects.GetAll";
        public const string GetById = "Library.Subjects.GetById";
        public const string GetList = "Library.Subjects.GetList";
        public const string GetNames = "Library.Subjects.GetNames";
        public const string Update = "Library.Subjects.Update";
    }

    public static class Users
    {
        public const string ChangePassword = "Library.Users.ChangePassword";
        public const string Create = "Library.Users.Create";
        public const string Delete = "Library.Users.Delete";
        public const string GetAll = "Library.Users.GetAll";
        public const string GetAllEntities = "Library.Users.GetAllEntities";
        public const string GetByUserName = "Library.Users.GetByUserName";
        public const string GetFullName = "Library.Users.GetFullName";
        public const string GetList = "Library.Users.GetList";
        public const string Revoke = "Library.Users.Revoke";
        public const string Update = "Library.Users.Update";
        public const string UpdateProfile = "Library.Users.UpdateProfile";
    }

    public static class Attachments
    {
        public const string Upload = "Library.Attachments.Upload";
        public const string Delete = "Library.Attachments.Delete";
        public const string GetByEntity = "Library.Attachments.GetByEntity";
        public const string Update = "Library.Attachments.Update";
    }

    public static class Menu
    {
        public const string GetAll = "Library.Menu.GetAll";
        public const string Create = "Library.Menu.Create";
        public const string Update = "Library.Menu.Update";
        public const string Delete = "Library.Menu.Delete";
        public const string Reorder = "Library.Menu.Reorder";
        public const string ResetToDefault = "Library.Menu.ResetToDefault";
    }

    public static class Visits
    {
        public const string GetStats = "Library.Visits.GetStats";
    }

    public static class Backup
    {
        public const string GetInfo = "Library.Backup.GetInfo";
        public const string Create = "Library.Backup.Create";
        public const string Download = "Library.Backup.Download";
        public const string Restore = "Library.Backup.Restore";
        public const string Cancel = "Library.Backup.Cancel";
    }

    public static class Reports
    {
        public const string BookSearchList = "Reports.BookSearchList";
        public const string PublicationSearchList = "Reports.PublicationSearchList";
        public const string ManuscriptSearchList = "Reports.ManuscriptSearchList";
        public const string ManuscriptStatistics = "Reports.ManuscriptStatistics";
        public const string BookStatistics = "Reports.BookStatistics";
        public const string PublicationStatistics = "Reports.PublicationStatistics";
        public const string Export = "Reports.Export";
        public const string View = "Reports.View";
    }
}
#pragma warning restore CA1034 

