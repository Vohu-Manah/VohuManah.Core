using Application.Abstractions.Messaging;
using Application.Library.Menu.GetAll;
using Application.Library.Settings;
using SharedKernel;
using Web.Api.Endpoints.Attributes;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Menu;

[RequireRole(EndpointNames.Menu.GetAll)]
internal sealed class GetAll : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/menu", async (
            IQueryHandler<GetAllMenuItemsQuery, List<MenuItemResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAllMenuItemsQuery();

            Result<List<MenuItemResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Menu");
    }
}

