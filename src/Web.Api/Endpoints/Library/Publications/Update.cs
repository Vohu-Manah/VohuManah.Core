using Application.Abstractions.Messaging;
using Application.Library.Publications.Update;
using SharedKernel;
using Web.Api.Endpoints.Attributes;
using Web.Api.Infrastructure;
using Application.Library.Settings;

namespace Web.Api.Endpoints.Library.Publications;

[RequireRole(EndpointNames.Publications.Update)]
internal sealed class Update : BaseEndpoint
{
    public sealed record Request(
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

    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("library/publications/{id:int}", async (
            int id,
            Request request,
            ICommandHandler<UpdatePublicationCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdatePublicationCommand(
                id,
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
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Publications");
    }
}

