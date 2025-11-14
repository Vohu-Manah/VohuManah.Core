using Application.Abstractions.Messaging;
using Application.Library.Manuscripts.Update;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Endpoints;
using Web.Api.Endpoints.Attributes;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Manuscripts;

[RequireRole("Library.Manuscripts.Update")]
internal sealed class Update : BaseEndpoint
{
    public sealed record Request(
        int Id,
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
        app.MapPut("library/manuscripts", async (
            Request request,
            ICommandHandler<UpdateManuscriptCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateManuscriptCommand(
                request.Id,
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

