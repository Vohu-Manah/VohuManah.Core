using Application.Abstractions.Messaging;
using Application.Library.Publications.Search;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Endpoints;
using Web.Api.Endpoints.Attributes;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Publications;

[RequireRole("Library.Publications.Search")]
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
        string? ToPeriod);

    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("library/publications/search", async (
            Request request,
            IQueryHandler<SearchPublicationsQuery, List<PublicationSearchResponse>> handler,
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
                request.ToPeriod);

            Result<List<PublicationSearchResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Publications");
    }
}

