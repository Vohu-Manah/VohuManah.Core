using Application.Abstractions.Messaging;
using Application.Library.Subjects.GetList;
using SharedKernel;
using Web.Api.Endpoints.Attributes;
using Web.Api.Extensions;
using Web.Api.Infrastructure;
using Application.Library.Settings;

namespace Web.Api.Endpoints.Library.Subjects;

[RequireRole(EndpointNames.Subjects.GetList)]
internal sealed class GetList : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/subjects/list", async (
            IQueryHandler<GetSubjectListQuery, List<SubjectListResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetSubjectListQuery();

            Result<List<SubjectListResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Subjects");
    }
}

