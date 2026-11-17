using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Publications.GetCountBySubject;
using VohuManah.Application.Library.Shared;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Infrastructure;
using VohuManah.Application.Abstractions.Authorization;

namespace VohuManah.Web.Api.Endpoints.Library.Publications;

[RequireRole(PermissionNames.Publications.GetCountBySubject)]
internal sealed class GetCountBySubject : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/publications/count-by-subject", async (
            IQueryHandler<GetPublicationCountBySubjectQuery, List<ListItemResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetPublicationCountBySubjectQuery();

            Result<List<ListItemResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Publications");
    }
}

