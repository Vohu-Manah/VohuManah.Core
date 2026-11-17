using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Publishers.GetAll;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Infrastructure;
using VohuManah.Application.Abstractions.Authorization;

namespace VohuManah.Web.Api.Endpoints.Library.Publishers;

[RequireRole(PermissionNames.Publishers.GetAll)]
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

