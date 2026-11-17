using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Publications.Search;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Http;
using VohuManah.Application.Abstractions.Authorization;

namespace VohuManah.Web.Api.Endpoints.Library.Publications;

[RequireRole(PermissionNames.Publications.Search)]
internal sealed class Search : BaseEndpoint
{
    public sealed record Request(
        string? Name,
        int PublicationTypeId,
        string? Concessionaire,
        string? ResponsibleDirector,
        string? Editor,
        string? FromYear,
        string? ToYear,
        string? No,
        string? PublishDate,
        int PublishPlaceId,
        string? JournalNo,
        int LanguageId,
        int SubjectId,
        string? FromPeriod,
        string? ToPeriod,
        int MaxResults = 5000,
        int Page = 1,
        int PageSize = 50);

    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("library/publications/search", async (
            Request request,
            IQueryHandler<SearchPublicationsQuery, PagedList<PublicationSearchResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new SearchPublicationsQuery(
                request.Name,
                request.PublicationTypeId,
                request.Concessionaire,
                request.ResponsibleDirector,
                request.Editor,
                request.FromYear,
                request.ToYear,
                request.No,
                request.PublishDate,
                request.PublishPlaceId,
                request.JournalNo,
                request.LanguageId,
                request.SubjectId,
                request.FromPeriod,
                request.ToPeriod, request.MaxResults, request.Page, request.PageSize);

            Result<PagedList<PublicationSearchResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Publications");
    }
}

