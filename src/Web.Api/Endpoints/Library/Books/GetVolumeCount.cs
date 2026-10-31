using Application.Abstractions.Messaging;
using Application.Library.Books.GetVolumeCount;
using SharedKernel;
using Web.Api.Endpoints.Attributes;
using Web.Api.Extensions;
using Web.Api.Infrastructure;
using Application.Library.Settings;

namespace Web.Api.Endpoints.Library.Books;

[RequireRole(EndpointNames.Books.GetVolumeCount)]
internal sealed class GetVolumeCount : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/books/volume-count", async (
            IQueryHandler<GetBookVolumeCountQuery, int> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetBookVolumeCountQuery();

            Result<int> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Books");
    }
}

