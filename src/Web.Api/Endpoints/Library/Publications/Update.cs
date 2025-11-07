using Application.Abstractions.Messaging;
using Application.Library.Publications.Update;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Publications;

internal sealed class Update : IEndpoint
{
    public sealed record Request(
        int Id,
        string Name,
        int TypeId,
        string Concessionaire,
        string ResponsibleDirector,
        string Editor,
        string Year,
        string JournalNo,
        string PublishDate,
        int PublishPlaceId,
        string No,
        string Period,
        int LanguageId,
        int SubjectId);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("library/publications", async (
            Request request,
            ICommandHandler<UpdatePublicationCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdatePublicationCommand(
                request.Id,
                request.Name,
                request.TypeId,
                request.Concessionaire,
                request.ResponsibleDirector,
                request.Editor,
                request.Year,
                request.JournalNo,
                request.PublishDate,
                request.PublishPlaceId,
                request.No,
                request.Period,
                request.LanguageId,
                request.SubjectId);

            Result result = await handler.Handle(command, cancellationToken);

            return result.IsSuccess ? Results.Ok() : CustomResults.Problem(result);
        })
        .WithTags("Library.Publications");
    }
}

