using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Manuscripts.GetAll;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Infrastructure;
using VohuManah.Application.Abstractions.Authorization;

namespace VohuManah.Web.Api.Endpoints.Library.Manuscripts;

[RequireRole(PermissionNames.Manuscripts.GetAll)]
internal sealed class GetAll : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/manuscripts", async (
            IQueryHandler<GetAllManuscriptsQuery, List<ManuscriptResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAllManuscriptsQuery();

            Result<List<ManuscriptResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Manuscripts");
    }
}

