using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.PublicationTypes.GetList;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Http;
using VohuManah.Application.Abstractions.Authorization;

namespace VohuManah.Web.Api.Endpoints.Library.PublicationTypes;

[RequireRole(PermissionNames.PublicationTypes.GetList)]
internal sealed class GetList : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/publication-types/list", async (
            IQueryHandler<GetPublicationTypeListQuery, List<PublicationTypeListResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetPublicationTypeListQuery();

            Result<List<PublicationTypeListResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.PublicationTypes");
    }
}

