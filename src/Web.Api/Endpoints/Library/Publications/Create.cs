using Application.Abstractions.Messaging;
using Application.Library.Publications.Create;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Endpoints;
using Web.Api.Endpoints.Attributes;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Publications;

[RequireRole("Library.Publications.Create")]
internal sealed class Create : BaseEndpoint
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
        app.MapPost("library/publications", async (
            Request request,
            ICommandHandler<CreatePublicationCommand, int> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreatePublicationCommand(
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

            Result<int> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Publications");
    }
}

