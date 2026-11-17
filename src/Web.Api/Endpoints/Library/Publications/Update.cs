using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Publications.Update;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Http;
using VohuManah.Application.Abstractions.Authorization;

namespace VohuManah.Web.Api.Endpoints.Library.Publications;

[RequireRole(PermissionNames.Publications.Update)]
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

