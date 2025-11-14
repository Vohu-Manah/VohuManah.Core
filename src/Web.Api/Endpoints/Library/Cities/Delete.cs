using Application.Abstractions.Messaging;
using Application.Library.Cities.Delete;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Endpoints;
using Web.Api.Endpoints.Attributes;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Cities;

[RequireRole("Library.Cities.Delete")]
internal sealed class Delete : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("library/cities/{id:int}", async (
            int id,
            ICommandHandler<DeleteCityCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new DeleteCityCommand(id);

            Result result = await handler.Handle(command, cancellationToken);

            return result.IsSuccess ? Results.Ok() : CustomResults.Problem(result);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Cities");
    }
}

