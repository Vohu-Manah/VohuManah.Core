using Application.Abstractions.Messaging;
using Application.Library.Books.Update;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Books;

internal sealed class Update : IEndpoint
{
    public sealed record Request(
        long Id,
        string Name,
        string Author,
        string Translator,
        string Editor,
        string Corrector,
        int PublisherId,
        int PublishPlaceId,
        string PublishYear,
        string PublishOrder,
        string Isbn,
        string No,
        int VolumeCount,
        int LanguageId,
        int SubjectId,
        string BookShelfRow);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("library/books", async (
            Request request,
            ICommandHandler<UpdateBookCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateBookCommand(
                request.Id,
                request.Name,
                request.Author,
                request.Translator,
                request.Editor,
                request.Corrector,
                request.PublisherId,
                request.PublishPlaceId,
                request.PublishYear,
                request.PublishOrder,
                request.Isbn,
                request.No,
                request.VolumeCount,
                request.LanguageId,
                request.SubjectId,
                request.BookShelfRow);

            Result result = await handler.Handle(command, cancellationToken);

            return result.IsSuccess ? Results.Ok() : CustomResults.Problem(result);
        })
        .WithTags("Library.Books");
    }
}

