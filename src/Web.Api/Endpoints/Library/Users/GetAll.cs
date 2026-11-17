using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Users.GetAll;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Http;
using VohuManah.Application.Abstractions.Authorization;

namespace VohuManah.Web.Api.Endpoints.Library.Users;

[RequireRole(PermissionNames.Users.GetAll)]
internal sealed class GetAll : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/users", async (
            IQueryHandler<GetAllLibraryUsersQuery, List<LibraryUserResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAllLibraryUsersQuery();

            Result<List<LibraryUserResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Users");
    }
}
