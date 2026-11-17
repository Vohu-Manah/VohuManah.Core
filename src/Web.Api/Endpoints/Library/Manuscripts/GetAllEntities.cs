using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Manuscripts.GetAllEntities;
using VohuManah.Domain.Manuscripts;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Infrastructure;
using VohuManah.Application.Library.Settings;

namespace VohuManah.Web.Api.Endpoints.Library.Manuscripts;

[RequireRole(EndpointNames.Manuscripts.GetAllEntities)]
internal sealed class GetAllEntities : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/manuscripts/entities", async (
            IQueryHandler<GetAllManuscriptEntitiesQuery, List<Manuscript>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAllManuscriptEntitiesQuery();

            Result<List<Manuscript>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Manuscripts");
    }
}

