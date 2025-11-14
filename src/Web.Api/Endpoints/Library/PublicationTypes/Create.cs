using Application.Abstractions.Messaging;
using Application.Library.PublicationTypes.Create;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Endpoints;
using Web.Api.Endpoints.Attributes;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.PublicationTypes;

[RequireRole("Library.PublicationTypes.Create")]
internal sealed class Create : BaseEndpoint
{
    public sealed record Request(string Title);

    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("library/publication-types", async (
            Request request,
            ICommandHandler<CreatePublicationTypeCommand, int> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreatePublicationTypeCommand(request.Title);

            Result<int> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.PublicationTypes");
    }
}

