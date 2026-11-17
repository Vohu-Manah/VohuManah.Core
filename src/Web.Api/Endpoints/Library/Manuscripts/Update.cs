using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Manuscripts.Update;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Http;
using VohuManah.Application.Abstractions.Authorization;

namespace VohuManah.Web.Api.Endpoints.Library.Manuscripts;

[RequireRole(PermissionNames.Manuscripts.Update)]
internal sealed class Update : BaseEndpoint
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

    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("library/manuscripts/{id:int}", async (
            int id,
            Request request,
            ICommandHandler<UpdateManuscriptCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateManuscriptCommand(
                id,
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

            Result result = await handler.Handle(command, cancellationToken);

            return result.IsSuccess ? Results.Ok() : CustomResults.Problem(result);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Manuscripts");
    }
}

