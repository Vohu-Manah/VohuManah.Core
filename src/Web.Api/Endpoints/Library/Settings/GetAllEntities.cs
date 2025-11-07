using Application.Abstractions.Messaging;
using Application.Library.Settings.GetAllEntities;
using Domain.Library;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Settings;

internal sealed class GetAllEntities : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/settings/entities", async (
            IQueryHandler<GetAllSettingsEntitiesQuery, List<Domain.Library.Settings>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAllSettingsEntitiesQuery();

            Result<List<Domain.Library.Settings>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags("Library.Settings");
    }
}

