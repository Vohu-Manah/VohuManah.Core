using Application.Abstractions.Messaging;
using Application.Library.Subjects.GetById;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Endpoints;
using Web.Api.Endpoints.Attributes;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Subjects;

[RequireRole("Library.Subjects.GetById")]
internal sealed class GetById : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
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
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Subjects");
    }
}

