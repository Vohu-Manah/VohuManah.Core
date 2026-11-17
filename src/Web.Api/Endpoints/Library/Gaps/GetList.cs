using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Gaps.GetList;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Infrastructure;
using VohuManah.Application.Library.Settings;

namespace VohuManah.Web.Api.Endpoints.Library.Gaps;

[RequireRole(EndpointNames.Gaps.GetList)]
internal sealed class GetList : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/gaps/list", async (
            IQueryHandler<GetGapListQuery, List<GapListResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetGapListQuery();

            Result<List<GapListResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Gaps");
    }
}

