using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Manuscripts.Create;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Http;
using VohuManah.Application.Abstractions.Authorization;

namespace VohuManah.Web.Api.Endpoints.Library.Manuscripts;

[RequireRole(PermissionNames.Manuscripts.Create)]
internal sealed class Create : BaseEndpoint
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
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Manuscripts");
    }
}

