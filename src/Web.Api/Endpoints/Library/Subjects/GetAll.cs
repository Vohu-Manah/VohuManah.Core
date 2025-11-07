using Application.Abstractions.Messaging;
using Application.Library.Subjects.GetAll;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Subjects;

internal sealed class GetAll : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/subjects", async (
            IQueryHandler<GetAllSubjectsQuery, List<SubjectResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAllSubjectsQuery();

            Result<List<SubjectResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags("Library.Subjects");
    }
}

