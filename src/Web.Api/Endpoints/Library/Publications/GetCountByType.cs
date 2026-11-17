using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Publications.GetCountByType;
using VohuManah.Application.Library.Shared;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Infrastructure;
using VohuManah.Application.Abstractions.Authorization;

namespace VohuManah.Web.Api.Endpoints.Library.Publications;

[RequireRole(PermissionNames.Publications.GetCountByType)]
internal sealed class GetCountByType : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/publications/count-by-type", async (
            IQueryHandler<GetPublicationCountByTypeQuery, List<ListItemResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetPublicationCountByTypeQuery();

            Result<List<ListItemResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Publications");
    }
}

