using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Cities.Delete;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Infrastructure;
using VohuManah.Application.Library.Settings;

namespace VohuManah.Web.Api.Endpoints.Library.Cities;

[RequireRole(EndpointNames.Cities.Delete)]
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

