using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Gaps.GetAll;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Infrastructure;
using VohuManah.Application.Abstractions.Authorization;

namespace VohuManah.Web.Api.Endpoints.Library.Gaps;

[RequireRole(PermissionNames.Gaps.GetAll)]
internal sealed class GetAll : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/gaps", async (
            IQueryHandler<GetAllGapsQuery, List<GapResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAllGapsQuery();

            Result<List<GapResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Gaps");
    }
}

