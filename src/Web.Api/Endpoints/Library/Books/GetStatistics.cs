using Application.Abstractions.Messaging;
using Application.Library.Books.GetStatistics;
using SharedKernel;
using Web.Api.Endpoints.Attributes;
using Web.Api.Extensions;
using Web.Api.Infrastructure;
using Application.Library.Settings;

namespace Web.Api.Endpoints.Library.Books;

[RequireRole(EndpointNames.Books.GetStatistics)]
internal sealed class GetStatistics : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/books/statistics", async (
            IQueryHandler<GetBookStatisticsQuery, BookStatisticsResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetBookStatisticsQuery();

            Result<BookStatisticsResponse> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Books");
    }
}

