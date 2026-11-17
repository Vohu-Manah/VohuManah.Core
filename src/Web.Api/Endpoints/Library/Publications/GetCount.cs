using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Publications.GetCount;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Infrastructure;
using VohuManah.Application.Library.Settings;

namespace VohuManah.Web.Api.Endpoints.Library.Publications;

[RequireRole(EndpointNames.Publications.GetCount)]
internal sealed class GetCount : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/publications/count", async (
            IQueryHandler<GetPublicationCountQuery, int> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetPublicationCountQuery();

            Result<int> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Publications");
    }
}

