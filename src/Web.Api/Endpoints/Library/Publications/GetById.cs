using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Publications.GetById;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Infrastructure;
using VohuManah.Application.Abstractions.Authorization;

namespace VohuManah.Web.Api.Endpoints.Library.Publications;

[RequireRole(PermissionNames.Publications.GetById)]
internal sealed class GetById : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/publications/{id:int}", async (
            int id,
            IQueryHandler<GetPublicationByIdQuery, PublicationResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetPublicationByIdQuery(id);

            Result<PublicationResponse> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Publications");
    }
}

