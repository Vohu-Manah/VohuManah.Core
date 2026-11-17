using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Subjects.Create;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Infrastructure;
using VohuManah.Application.Library.Settings;

namespace VohuManah.Web.Api.Endpoints.Library.Subjects;

[RequireRole(EndpointNames.Subjects.Create)]
internal sealed class Create : BaseEndpoint
{
    public sealed record Request(string Title);

    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("library/subjects", async (
            Request request,
            ICommandHandler<CreateSubjectCommand, int> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateSubjectCommand(request.Title);

            Result<int> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Subjects");
    }
}

