using Application.Abstractions.Messaging;
using Application.Library.Publishers.GetAll;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Endpoints;
using Web.Api.Endpoints.Attributes;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Publishers;

[RequireRole("Library.Publishers.GetAll")]
internal sealed class GetAll : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/publishers", async (
            IQueryHandler<GetAllPublishersQuery, List<PublisherResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAllPublishersQuery();

            Result<List<PublisherResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Publishers");
    }
}

