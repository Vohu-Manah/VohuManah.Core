using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Cities.Create;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Infrastructure;
using VohuManah.Application.Abstractions.Authorization;

namespace VohuManah.Web.Api.Endpoints.Library.Cities;

[RequireRole(PermissionNames.Cities.Create)]
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

