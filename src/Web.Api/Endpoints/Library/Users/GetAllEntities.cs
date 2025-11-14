using Application.Abstractions.Messaging;
using Application.Library.Users.GetAllEntities;
using Domain.Library;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Endpoints;
using Web.Api.Endpoints.Attributes;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Users;

[RequireRole("Library.Users.GetAllEntities")]
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

