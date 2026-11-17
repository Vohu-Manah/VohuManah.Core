using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Manuscripts.GetById;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Infrastructure;
using VohuManah.Application.Abstractions.Authorization;

namespace VohuManah.Web.Api.Endpoints.Library.Manuscripts;

[RequireRole(PermissionNames.Manuscripts.GetById)]
internal sealed class GetById : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/manuscripts/{id:int}", async (
            int id,
            IQueryHandler<GetManuscriptByIdQuery, ManuscriptResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetManuscriptByIdQuery(id);

            Result<ManuscriptResponse> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Manuscripts");
    }
}

