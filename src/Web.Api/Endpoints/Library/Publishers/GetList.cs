using Application.Abstractions.Messaging;
using Application.Library.Publishers.GetList;
using SharedKernel;
using Web.Api.Endpoints.Attributes;
using Web.Api.Extensions;
using Web.Api.Infrastructure;
using Application.Library.Settings;

namespace Web.Api.Endpoints.Library.Publishers;

[RequireRole(EndpointNames.Publishers.GetList)]
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

