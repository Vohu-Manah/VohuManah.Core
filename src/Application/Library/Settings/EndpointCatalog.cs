using System;
using System.Collections.Immutable;
using System.Linq;

namespace Application.Library.Settings;

/// <summary>
/// Central registry of all secured endpoints (those decorated with role requirements).
/// Keeping the list in one place lets us validate role-permission assignments at runtime and drive seed data.
/// </summary>
public static class EndpointCatalog
{
    public static readonly ImmutableArray<string> Books = ImmutableArray.Create(
        "Library.Books.Create",
        "Library.Books.Delete",
        "Library.Books.GetAll",
        "Library.Books.GetAllEntities",
        "Library.Books.GetById",
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
        "Library.Settings.GetAllRoles",
        "Library.Settings.GetBackground",
        "Library.Settings.GetById",
        "Library.Settings.GetByName",
        "Library.Settings.GetMainTitle",
        "Library.Settings.GetUserRoles",
        "Library.Settings.RemoveRoleFromUser",
        "Library.Settings.Update",
        "Library.Settings.UpdateRoleEndpoints");

    public static readonly ImmutableArray<string> Subjects = ImmutableArray.Create(
        "Library.Subjects.Create",
        "Library.Subjects.Delete",
        "Library.Subjects.GetAll",
        "Library.Subjects.GetById",
        "Library.Subjects.GetList",
        "Library.Subjects.GetNames",
        "Library.Subjects.Update");

    public static readonly ImmutableArray<string> Users = ImmutableArray.Create(
        "Library.Users.Create",
        "Library.Users.Delete",
        "Library.Users.GetAll",
        "Library.Users.GetAllEntities",
        "Library.Users.GetByUserName",
        "Library.Users.GetFullName",
        "Library.Users.GetList",
        "Library.Users.Revoke",
        "Library.Users.Update");

    public static readonly ImmutableArray<string> Attachments = ImmutableArray.Create(
        "Library.Attachments.Upload",
        "Library.Attachments.Delete",
        "Library.Attachments.GetByEntity");

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
            .Concat(Attachments));

    public static readonly ImmutableHashSet<string> All = Combined.ToImmutableHashSet(StringComparer.OrdinalIgnoreCase);

    public static bool IsKnownEndpoint(string endpointName) =>
        !string.IsNullOrWhiteSpace(endpointName) && All.Contains(endpointName);
}

