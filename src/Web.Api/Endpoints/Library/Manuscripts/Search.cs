using Application.Abstractions.Messaging;
using Application.Library.Manuscripts.Search;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Manuscripts;

internal sealed class Search : IEndpoint
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
        string? Size);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("library/manuscripts/search", async (
            Request request,
            IQueryHandler<SearchManuscriptsQuery, List<ManuscriptSearchResponse>> handler,
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
                request.Size);

            Result<List<ManuscriptSearchResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags("Library.Manuscripts");
    }
}

