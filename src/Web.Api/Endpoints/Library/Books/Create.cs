using Application.Abstractions.Messaging;
using Application.Library.Books.Create;
using Application.Library.Settings;
using SharedKernel;
using Web.Api.Endpoints.Attributes;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Books;

[RequireRole(EndpointNames.Books.Create)]
internal sealed class Create : BaseEndpoint
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
        app.MapPost("library/books", async (
            Request request,
            ICommandHandler<CreateBookCommand, long> handler,
            CancellationToken cancellationToken) =>
        {
            ValidationError? typeValidationError = ModelBindingValidator.ValidateRequestTypes(request);
            if (typeValidationError != null)
            {
                return CustomResults.Problem(Result.Failure(typeValidationError));
            }

            var command = new CreateBookCommand(
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

            Result<long> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ValidateRequestTypes<Request>()
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Books");
    }
}
