using Application.Abstractions.Messaging;
using Application.Library.Publications.GetAll;
using SharedKernel;
using Web.Api.Endpoints.Attributes;
using Web.Api.Extensions;
using Web.Api.Infrastructure;
using Application.Library.Settings;

namespace Web.Api.Endpoints.Library.Publications;

[RequireRole(EndpointNames.Publications.GetAll)]
internal sealed class GetAll : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/publications", async (
            IQueryHandler<GetAllPublicationsQuery, List<PublicationResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAllPublicationsQuery();

            Result<List<PublicationResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Publications");
    }
}

