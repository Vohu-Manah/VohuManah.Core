using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Publishers.Create;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Http;
using VohuManah.Application.Abstractions.Authorization;

namespace VohuManah.Web.Api.Endpoints.Library.Publishers;

[RequireRole(PermissionNames.Publishers.Create)]
internal sealed class Create : BaseEndpoint
{
    public sealed record Request(
        string Name,
        string ManagerName,
        int PlaceId,
        string Address,
        string Telephone,
        string Website,
        string Email);

    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("library/publishers", async (
            Request request,
            ICommandHandler<CreatePublisherCommand, int> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreatePublisherCommand(
                request.Name,
                request.ManagerName,
                request.PlaceId,
                request.Address,
                request.Telephone,
                request.Website,
                request.Email);

            Result<int> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Publishers");
    }
}

