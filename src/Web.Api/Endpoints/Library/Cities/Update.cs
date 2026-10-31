using Application.Abstractions.Messaging;
using Application.Library.Cities.Update;
using SharedKernel;
using Web.Api.Endpoints.Attributes;
using Web.Api.Infrastructure;
using Application.Library.Settings;

namespace Web.Api.Endpoints.Library.Cities;

[RequireRole(EndpointNames.Cities.Update)]
internal sealed class Update : BaseEndpoint
{
    public sealed record Request(string Name);

    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("library/cities/{id:int}", async (
            int id,
            Request request,
            ICommandHandler<UpdateCityCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateCityCommand(id, request.Name);

            Result result = await handler.Handle(command, cancellationToken);

            return result.IsSuccess ? Results.Ok() : CustomResults.Problem(result);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Cities");
    }
}

