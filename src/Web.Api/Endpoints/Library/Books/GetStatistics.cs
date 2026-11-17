using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Books.GetStatistics;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Infrastructure;
using VohuManah.Application.Library.Settings;

namespace VohuManah.Web.Api.Endpoints.Library.Books;

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

