using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Users.GetByUserName;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Http;
using VohuManah.Application.Abstractions.Authorization;

namespace VohuManah.Web.Api.Endpoints.Library.Users;

[RequireRole(PermissionNames.Users.GetByUserName)]
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
