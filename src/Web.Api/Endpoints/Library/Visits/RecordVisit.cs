using Application.Abstractions.Messaging;
using Application.Library.Visits.Record;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Visits;



internal sealed class RecordVisit : BaseEndpoint
{
    public sealed record Request(string Path);

    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("library/visits", async (
            Request request,
            ICommandHandler<RecordVisitCommand> handler,
            CancellationToken cancellationToken) =>
        {
            Result result = await handler.Handle(new RecordVisitCommand(request.Path), cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Visits");
    }
}