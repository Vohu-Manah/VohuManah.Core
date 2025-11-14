using Application.Abstractions.Messaging;
using Application.Library.Languages.Update;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Endpoints;
using Web.Api.Endpoints.Attributes;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Languages;

[RequireRole("Library.Languages.Update")]
internal sealed class Update : BaseEndpoint
{
    public sealed record Request(int Id, string Name, string Abbreviation);

    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("library/languages", async (
            Request request,
            ICommandHandler<UpdateLanguageCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateLanguageCommand(request.Id, request.Name, request.Abbreviation);

            Result result = await handler.Handle(command, cancellationToken);

            return result.IsSuccess ? Results.Ok() : CustomResults.Problem(result);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Languages");
    }
}

