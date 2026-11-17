using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Publications.GetAllEntities;
using VohuManah.Domain.Library;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Infrastructure;
using VohuManah.Application.Library.Settings;

namespace VohuManah.Web.Api.Endpoints.Library.Publications;

[RequireRole(EndpointNames.Publications.GetAllEntities)]
internal sealed class GetAllEntities : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/publications/entities", async (
            IQueryHandler<GetAllPublicationEntitiesQuery, List<Publication>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAllPublicationEntitiesQuery();

            Result<List<Publication>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Publications");
    }
}

