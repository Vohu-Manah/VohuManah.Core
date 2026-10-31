using Application.Abstractions.Messaging;
using Application.Library.Subjects.GetAll;
using SharedKernel;
using Web.Api.Endpoints.Attributes;
using Web.Api.Extensions;
using Web.Api.Infrastructure;
using Application.Library.Settings;

namespace Web.Api.Endpoints.Library.Subjects;

[RequireRole(EndpointNames.Subjects.GetAll)]
internal sealed class GetAll : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/subjects", async (
            IQueryHandler<GetAllSubjectsQuery, List<SubjectResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAllSubjectsQuery();

            Result<List<SubjectResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Subjects");
    }
}

