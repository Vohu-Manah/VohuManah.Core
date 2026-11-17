using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Subjects.GetById;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Infrastructure;
using VohuManah.Application.Library.Settings;

namespace VohuManah.Web.Api.Endpoints.Library.Subjects;

[RequireRole(EndpointNames.Subjects.GetById)]
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

