using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Manuscripts.GetCountByYear;
using VohuManah.Application.Library.Shared;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Infrastructure;
using VohuManah.Application.Abstractions.Authorization;

namespace VohuManah.Web.Api.Endpoints.Library.Manuscripts;

[RequireRole(PermissionNames.Manuscripts.GetCountByYear)]
internal sealed class GetCountByYear : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/manuscripts/count-by-year", async (
            IQueryHandler<GetManuscriptCountByYearQuery, List<ListItemResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetManuscriptCountByYearQuery();

            Result<List<ListItemResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Manuscripts");
    }
}

