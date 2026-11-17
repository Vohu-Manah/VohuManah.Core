using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Languages.Update;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Infrastructure;
using VohuManah.Application.Library.Settings;

namespace VohuManah.Web.Api.Endpoints.Library.Languages;

[RequireRole(EndpointNames.Languages.Update)]
internal sealed class Update : BaseEndpoint
{
    public sealed record Request(string Name, string Abbreviation);

    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("library/languages/{id:int}", async (
            int id,
            Request request,
            ICommandHandler<UpdateLanguageCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateLanguageCommand(id, request.Name, request.Abbreviation);

            Result result = await handler.Handle(command, cancellationToken);

            return result.IsSuccess ? Results.Ok() : CustomResults.Problem(result);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Languages");
    }
}

