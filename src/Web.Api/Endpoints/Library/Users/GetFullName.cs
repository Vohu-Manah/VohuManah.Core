using Application.Abstractions.Messaging;
using Application.Library.Users.GetFullName;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Endpoints;
using Web.Api.Endpoints.Attributes;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Users;

[RequireRole("Library.Users.GetFullName")]
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

