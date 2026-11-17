using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Manuscripts.GetCount;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Infrastructure;
using VohuManah.Application.Abstractions.Authorization;

namespace VohuManah.Web.Api.Endpoints.Library.Manuscripts;

[RequireRole(PermissionNames.Manuscripts.GetCount)]
internal sealed class GetCount : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/manuscripts/count", async (
            IQueryHandler<GetManuscriptCountQuery, int> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetManuscriptCountQuery();

            Result<int> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Manuscripts");
    }
}

