using Application.Abstractions.Messaging;
using Application.Library.Manuscripts.Delete;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Endpoints;
using Web.Api.Endpoints.Attributes;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Manuscripts;

[RequireRole("Library.Manuscripts.Delete")]
internal sealed class Delete : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("library/manuscripts/{id:int}", async (
            int id,
            ICommandHandler<DeleteManuscriptCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new DeleteManuscriptCommand(id);

            Result result = await handler.Handle(command, cancellationToken);

            return result.IsSuccess ? Results.Ok() : CustomResults.Problem(result);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Manuscripts");
    }
}

