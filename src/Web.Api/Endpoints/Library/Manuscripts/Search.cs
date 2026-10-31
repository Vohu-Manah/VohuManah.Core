using Application.Abstractions.Messaging;
using Application.Library.Shared;
using Application.Library.Manuscripts.Search;
using SharedKernel;
using Web.Api.Endpoints.Attributes;
using Web.Api.Extensions;
using Web.Api.Infrastructure;
using Application.Library.Settings;

namespace Web.Api.Endpoints.Library.Manuscripts;

[RequireRole(EndpointNames.Manuscripts.Search)]
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

