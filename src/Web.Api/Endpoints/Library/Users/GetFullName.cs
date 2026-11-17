using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Users.GetFullName;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Http;
using VohuManah.Application.Abstractions.Authorization;

namespace VohuManah.Web.Api.Endpoints.Library.Users;

[RequireRole(PermissionNames.Users.GetFullName)]
internal sealed class GetFullName : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/users/{userName}/fullname", async (
            string userName,
            IQueryHandler<GetUserFullNameQuery, string?> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetUserFullNameQuery(userName);

            Result<string?> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Users");
    }
}

