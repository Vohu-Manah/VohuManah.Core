using Application.Abstractions.Messaging;
using Application.Library.Subjects.GetById;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Subjects;

internal sealed class GetById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/subjects/{id:int}", async (
            int id,
            IQueryHandler<GetSubjectByIdQuery, SubjectResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetSubjectByIdQuery(id);

            Result<SubjectResponse> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags("Library.Subjects");
    }
}

