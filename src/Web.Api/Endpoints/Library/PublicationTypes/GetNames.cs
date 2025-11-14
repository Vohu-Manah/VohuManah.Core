using Application.Abstractions.Messaging;
using Application.Library.PublicationTypes.GetNames;
using Application.Library._Shared;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Endpoints;
using Web.Api.Endpoints.Attributes;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.PublicationTypes;

[RequireRole("Library.PublicationTypes.GetNames")]
internal sealed class GetNames : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/publication-types/names", async (
            bool? addAll,
            IQueryHandler<GetPublicationTypeNamesQuery, List<SelectItemResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetPublicationTypeNamesQuery(addAll ?? false);
            Result<List<SelectItemResponse>> result = await handler.Handle(query, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.PublicationTypes");
    }
}


