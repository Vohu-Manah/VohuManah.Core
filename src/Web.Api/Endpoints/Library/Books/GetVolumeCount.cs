using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Books.GetVolumeCount;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Infrastructure;
using VohuManah.Application.Abstractions.Authorization;

namespace VohuManah.Web.Api.Endpoints.Library.Books;

[RequireRole(PermissionNames.Books.GetVolumeCount)]
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

