using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.PublicationTypes.GetById;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Http;
using VohuManah.Application.Abstractions.Authorization;

namespace VohuManah.Web.Api.Endpoints.Library.PublicationTypes;

[RequireRole(PermissionNames.PublicationTypes.GetById)]
internal sealed class GetById : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/publication-types/{id:int}", async (
            int id,
            IQueryHandler<GetPublicationTypeByIdQuery, PublicationTypeResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetPublicationTypeByIdQuery(id);

            Result<PublicationTypeResponse> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.PublicationTypes");
    }
}

