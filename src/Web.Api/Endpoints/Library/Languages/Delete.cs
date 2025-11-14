using Application.Abstractions.Messaging;
using Application.Library.Languages.Delete;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Endpoints;
using Web.Api.Endpoints.Attributes;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Languages;

[RequireRole("Library.Languages.Delete")]
internal sealed class Delete : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("library/languages/{id:int}", async (
            int id,
            ICommandHandler<DeleteLanguageCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new DeleteLanguageCommand(id);

            Result result = await handler.Handle(command, cancellationToken);

            return result.IsSuccess ? Results.Ok() : CustomResults.Problem(result);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Languages");
    }
}

