using Application.Abstractions.Messaging;
using Application.Library.Shared;
using Application.Library.Publications.Search;
using SharedKernel;
using Web.Api.Endpoints.Attributes;
using Web.Api.Extensions;
using Web.Api.Infrastructure;
using Application.Library.Settings;

namespace Web.Api.Endpoints.Library.Publications;

[RequireRole(EndpointNames.Publications.Search)]
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
            IQueryHandler<SearchPublicationsQuery, PagedResult<PublicationSearchResponse>> handler,
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

            Result<PagedResult<PublicationSearchResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Publications");
    }
}

