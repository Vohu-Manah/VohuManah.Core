using Application.Abstractions.Messaging;
using Application.Library.Users.GetByUserName;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Endpoints;
using Web.Api.Endpoints.Attributes;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Users;

[RequireRole("Library.Users.GetByUserName")]
internal sealed class GetByUserName : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/users/{userName}", async (
            string userName,
            IQueryHandler<GetLibraryUserByUserNameQuery, LibraryUserResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetLibraryUserByUserNameQuery(userName);

            Result<LibraryUserResponse> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Users");
    }
}
