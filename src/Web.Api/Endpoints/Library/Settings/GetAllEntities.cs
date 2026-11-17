using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Settings.GetAllEntities;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Infrastructure;
using VohuManah.Application.Library.Settings;

namespace VohuManah.Web.Api.Endpoints.Library.Settings;

[RequireRole(EndpointNames.Settings.GetAllEntities)]
internal sealed class GetAllEntities : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/settings/entities", async (
            IQueryHandler<GetAllSettingsEntitiesQuery, List<VohuManah.Domain.Library.Settings>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAllSettingsEntitiesQuery();

            Result<List<VohuManah.Domain.Library.Settings>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Settings");
    }
}

