using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Publishers.GetList;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Infrastructure;
using VohuManah.Application.Abstractions.Authorization;

namespace VohuManah.Web.Api.Endpoints.Library.Publishers;

[RequireRole(PermissionNames.Publishers.GetList)]
internal sealed class GetList : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/publishers/list", async (
            IQueryHandler<GetPublisherListQuery, List<PublisherListResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetPublisherListQuery();

            Result<List<PublisherListResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Publishers");
    }
}

