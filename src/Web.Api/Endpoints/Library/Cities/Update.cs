using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Cities.Update;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Infrastructure;
using VohuManah.Application.Library.Settings;

namespace VohuManah.Web.Api.Endpoints.Library.Cities;

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

