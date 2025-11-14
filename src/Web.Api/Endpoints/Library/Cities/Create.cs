using Application.Abstractions.Messaging;
using Application.Library.Cities.Create;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Endpoints;
using Web.Api.Endpoints.Attributes;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Cities;

[RequireRole("Library.Cities.Create")]
internal sealed class Create : BaseEndpoint
{
    public sealed record Request(string Name);

    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("library/cities", async (
            Request request,
            ICommandHandler<CreateCityCommand, int> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateCityCommand(request.Name);

            Result<int> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Cities");
    }
}

