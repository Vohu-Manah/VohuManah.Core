using System.Security.Claims;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Menu.GetAll;
using VohuManah.Application.Abstractions.Authorization;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Infrastructure;

namespace VohuManah.Web.Api.Endpoints.Library.Menu;

[RequireRole(PermissionNames.Menu.GetAll)]
internal sealed class GetAll : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/menu", async (
            HttpContext httpContext,
            IQueryHandler<GetAllMenuItemsQuery, List<MenuItemResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var roles = httpContext.User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var query = new GetAllMenuItemsQuery(roles);

            Result<List<MenuItemResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Menu");
    }
}

