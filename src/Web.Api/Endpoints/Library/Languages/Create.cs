using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Languages.Create;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Http;
using VohuManah.Application.Abstractions.Authorization;

namespace VohuManah.Web.Api.Endpoints.Library.Languages;

[RequireRole(PermissionNames.Languages.Create)]
internal sealed class Create : BaseEndpoint
{
    public sealed record Request(string Name, string Abbreviation);

    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("library/languages", async (
            Request request,
            ICommandHandler<CreateLanguageCommand, int> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateLanguageCommand(request.Name, request.Abbreviation);

            Result<int> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Languages");
    }
}

