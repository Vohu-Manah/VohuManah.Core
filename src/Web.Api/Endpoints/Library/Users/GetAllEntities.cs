using Application.Abstractions.Messaging;
using Application.Library.Users.GetAllEntities;
using Domain.Library;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Users;

internal sealed class GetAllEntities : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/users/entities", async (
            IQueryHandler<GetAllUserEntitiesQuery, List<User>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAllUserEntitiesQuery();

            Result<List<User>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags("Library.Users");
    }
}

