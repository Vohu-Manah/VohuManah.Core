using Application.Abstractions.Messaging;
using Application.Library.Books.Update;
using Application.Library.Settings;
using SharedKernel;
using Web.Api.Endpoints.Attributes;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Books;

[RequireRole(EndpointNames.Books.Update)]
internal sealed class Update : BaseEndpoint
{
    public sealed record Request(
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

    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("library/books/{id:long}", async (
            long id,
            Request request,
            ICommandHandler<UpdateBookCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateBookCommand(
                id,
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
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Books");
    }
}

