using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Books.GetCount;
using VohuManah.Application.Library.Settings;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Infrastructure;

namespace VohuManah.Web.Api.Endpoints.Library.Books;

[RequireRole(EndpointNames.Books.GetCount)]
internal sealed class GetCount : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/books/count", async (
            IQueryHandler<GetBookCountQuery, int> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetBookCountQuery();

            Result<int> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Books");
    }
}

