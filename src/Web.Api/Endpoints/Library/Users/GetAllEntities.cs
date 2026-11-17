using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Users.GetAllEntities;
using VohuManah.Domain.Users;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Infrastructure;
using VohuManah.Application.Library.Settings;

namespace VohuManah.Web.Api.Endpoints.Library.Users;

[RequireRole(EndpointNames.Users.GetAllEntities)]
internal sealed class GetAllEntities : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/users/entities", async (
            IQueryHandler<GetAllUserEntitiesQuery, List<User>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAllUserEntitiesQuery();

            Result<List<User>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Users");
    }
}

