using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Settings;
using VohuManah.Application.Library.Settings.GetAllEndpoints;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Infrastructure;

namespace VohuManah.Web.Api.Endpoints.Library.Settings;

[RequireRole(EndpointNames.Settings.GetAllEndpoints)]
internal sealed class GetAllEndpoints : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/settings/endpoints", async (
            IQueryHandler<GetAllEndpointsQuery, Dictionary<string, List<string>>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAllEndpointsQuery();

            Result<Dictionary<string, List<string>>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Settings");
    }
}

