using Application.Abstractions.Messaging;
using Application.Library.Manuscripts.Create;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Manuscripts;

internal sealed class Create : IEndpoint
{
    public sealed record Request(
        string Name,
        string Author,
        int WritingPlaceId,
        string Year,
        int PageCount,
        string Size,
        int GapId,
        string VersionNo,
        int LanguageId,
        int SubjectId);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("library/manuscripts", async (
            Request request,
            ICommandHandler<CreateManuscriptCommand, int> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateManuscriptCommand(
                request.Name,
                request.Author,
                request.WritingPlaceId,
                request.Year,
                request.PageCount,
                request.Size,
                request.GapId,
                request.VersionNo,
                request.LanguageId,
                request.SubjectId);

            Result<int> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags("Library.Manuscripts");
    }
}

