using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Shared;
using VohuManah.Application.Library.Manuscripts.Search;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Infrastructure;
using VohuManah.Application.Abstractions.Authorization;

namespace VohuManah.Web.Api.Endpoints.Library.Manuscripts;

[RequireRole(PermissionNames.Manuscripts.Search)]
internal sealed class Search : BaseEndpoint
{
    public sealed record Request(
        string? Name,
        string? Author,
        int WritingPlaceId,
        string? FromYear,
        string? ToYear,
        int FromPageCount,
        int ToPageCount,
        string? No,
        int LanguageId,
        int SubjectId,
        int GapId,
        string? Size,
        int MaxResults = 5000,
        int Page = 1,
        int PageSize = 50);

    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("library/manuscripts/search", async (
            Request request,
            IQueryHandler<SearchManuscriptsQuery, PagedResult<ManuscriptSearchResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new SearchManuscriptsQuery(
                request.Name,
                request.Author,
                request.WritingPlaceId,
                request.FromYear,
                request.ToYear,
                request.FromPageCount,
                request.ToPageCount,
                request.No,
                request.LanguageId,
                request.SubjectId,
                request.GapId,
                request.Size, request.MaxResults, request.Page, request.PageSize);

            Result<PagedResult<ManuscriptSearchResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Manuscripts");
    }
}

