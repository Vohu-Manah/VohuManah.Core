using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Subjects.GetList;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Infrastructure;
using VohuManah.Application.Abstractions.Authorization;

namespace VohuManah.Web.Api.Endpoints.Library.Subjects;

[RequireRole(PermissionNames.Subjects.GetList)]
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

